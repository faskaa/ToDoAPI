using Microsoft.AspNetCore.Identity;

namespace ToDoApp.Entities
{
    public class CustomUser : IdentityUser
    {
        public List<Todo> Todos { get; set; }
    }
}
    