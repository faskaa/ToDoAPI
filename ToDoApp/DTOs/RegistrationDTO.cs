using System.ComponentModel.DataAnnotations;

namespace ToDoApp.DTOs
{
    public class RegistrationDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }

    }
}
