using azure_function_crud.Dtos;
using azure_function_crud.Models;

namespace azure_function_crud
{
    public static class Mapping
    {
        public static ToDo CreateToDoFromDto(this ToDoDto dto)
        {
            return new ToDo
            {
                Name = dto.Name,
                IsDone = dto.IsDone
            };
        }
    }
}
