using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using azure_function_crud.Dtos;
using azure_function_crud.Models;
using FastMember;

namespace azure_function_crud
{
    public static class Repository
    {
        private static string ConnectionString => Environment.GetEnvironmentVariable("ConnectionString");
        public static async Task<List<ToDo>> GetAllAsync()
        {
            var todosList = new List<ToDo>();

            using (var connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();

                const string command = "SELECT Id, Name, IsDone FROM ToDos";

                using (var cmd = new SqlCommand(command, connection))
                {
                    var reader = await cmd.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            todosList.Add(reader.ConvertToObject<ToDo>());
                        }
                    }
                }
            }

            return todosList;
        }

        public static async Task<ToDo> GetTodoAsync(string id)
        {
            var todo = new ToDo();

            using (var connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();

                const string command = "SELECT Id, Name, IsDone FROM ToDos WHERE Id = @ID";

                using (var cmd = new SqlCommand(command, connection))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow);

                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        todo = reader.ConvertToObject<ToDo>();
                    }
                }
            }

            return todo;
        }

        public static async Task<bool> DeleteTodoAsync(string id)
        {
            int rows;

            using (var connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();

                const string command = "DELETE FROM ToDos WHERE Id = @ID";

                using (var cmd = new SqlCommand(command, connection))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    rows = await cmd.ExecuteNonQueryAsync();
                }
            }

            return rows != 0;
        }

        public static async Task<ToDo> CreateTodo(ToDoDto dto)
        {
            var todo = dto.CreateToDoFromDto();
            int rows;

            using (var connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();

                const string command = "INSERT INTO ToDos (Id, Name, IsDone) VALUES (@ID, @NAME, @ISDONE)";

                using (var cmd = new SqlCommand(command, connection))
                {
                    cmd.Parameters.AddWithValue("@ID", todo.Id);
                    cmd.Parameters.AddWithValue("@NAME", todo.Name);
                    cmd.Parameters.AddWithValue("@ISDONE", todo.IsDone);

                    rows = await cmd.ExecuteNonQueryAsync();
                }
            }

            return rows != 0 ? todo : null;
        }

        public static async Task<ToDo> UpdateToDoAsync(ToDoDto dto)
        {
            var toUpdate = await GetTodoAsync(dto.Id);
            int rows;

            if (toUpdate == null)
            {
                return null;
            }

            toUpdate.IsDone = dto.IsDone;
            if (!string.IsNullOrEmpty(dto.Name))
            {
                toUpdate.Name = dto.Name;
            }

            using (var connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();

                const string command = "UPDATE ToDos SET Name = @NAME, IsDone = @ISDONE WHERE Id = @ID";

                using (var cmd = new SqlCommand(command, connection))
                {
                    cmd.Parameters.AddWithValue("@ID", toUpdate.Id);
                    cmd.Parameters.AddWithValue("@NAME", toUpdate.Name);
                    cmd.Parameters.AddWithValue("@ISDONE", toUpdate.IsDone);

                    rows = await cmd.ExecuteNonQueryAsync();
                }
            }

            return rows != 0 ? toUpdate : null;
        }

        private static T ConvertToObject<T>(this SqlDataReader rd) where T : class, new()
        {
            var type = typeof(T);
            var accessor = TypeAccessor.Create(type);
            var members = accessor.GetMembers();
            var t = new T();

            for (var i = 0; i < rd.FieldCount; i++)
            {
                if (!rd.IsDBNull(i))
                {
                    var fieldName = rd.GetName(i);

                    if (members.Any(m => string.Equals(m.Name, fieldName, StringComparison.OrdinalIgnoreCase)))
                    {
                        accessor[t, fieldName] = rd.GetValue(i);
                    }
                }
            }

            return t;
        }
    }
}
