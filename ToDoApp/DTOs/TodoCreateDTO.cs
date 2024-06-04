using System.ComponentModel.DataAnnotations;

namespace ToDoApp.DTOs
{
    public class TodoCreateDTO
    {
        [Required]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
