using System.IO;
using System.Threading.Tasks;
using azure_function_crud.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace azure_function_crud
{
    public static class Api
    {
        [FunctionName("GetAllTodos")]
        public static async Task<IActionResult> GetAllTodos(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo")] HttpRequest req,
            ILogger log)
        {
            var result = await Repository.GetAllAsync();

            return new OkObjectResult(result);
        }

        [FunctionName("GetTodo")]
        public static async Task<IActionResult> GetTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo/{id}")]
            HttpRequest req, string id, ILogger log)
        {
            var result = await Repository.GetTodoAsync(id);

            if (result == null)
            {
                return new BadRequestResult();
            }

            return new OkObjectResult(result);
        }

        [FunctionName("DeteteTodo")]
        public static async Task<IActionResult> DeleteTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "todo/{id}")]
            HttpRequest req, string id, ILogger log)
        {
            if (await Repository.DeleteTodoAsync(id))
            {
                return new NoContentResult();
            }
            return new BadRequestResult();
        }

        [FunctionName("CreateTodo")]
        public static async Task<IActionResult> CreateTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todo")]
            HttpRequest req, ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<ToDoDto>(requestBody);

            var result = await Repository.CreateTodo(data);

            if (result == null)
            {
                return new BadRequestResult();
            }

            return new OkObjectResult(result);
        }

        [FunctionName("UpdateTodo")]
        public static async Task<IActionResult> UpdateTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "todo/{id}")]
            HttpRequest req, string id, ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<ToDoDto>(requestBody);
            data.Id = id;

            var result = await Repository.UpdateToDoAsync(data);

            if (result == null)
            {
                return new BadRequestResult();
            }

            return new OkObjectResult(result);
        }
    }
}
