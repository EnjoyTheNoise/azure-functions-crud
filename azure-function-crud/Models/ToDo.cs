using System;

namespace azure_function_crud.Models
{
    public class ToDo
    {
        public string Id = Guid.NewGuid().ToString("N");
        public string Name { get; set; }
        public bool IsDone { get; set; }
    }
}
