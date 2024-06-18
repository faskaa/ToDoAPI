using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoApp.DTOs
{
    public class TodoUpdateDTO
    {
        public string? Name { get; set; }
        public bool? Completed { get; set; }
    }
}