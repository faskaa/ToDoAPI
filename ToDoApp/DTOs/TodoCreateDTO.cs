using System.ComponentModel.DataAnnotations;

namespace ToDoApp.DTOs
{
    public class TodoCreateDTO
    {
        [Required]
        public string Name { get; set; }
        public bool Completed { get; set; }

    }
}