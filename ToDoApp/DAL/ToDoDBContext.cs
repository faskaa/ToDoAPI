using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Entities;

namespace ToDoApp.DAL
{
    public class ToDoDBContext : IdentityDbContext<CustomUser>
    {
        public ToDoDBContext(DbContextOptions<ToDoDBContext> options) : base(options)
        {
            
        }

        public DbSet<Todo> Todos { get; set; }
        public DbSet<CustomUser> CustomUsers     { get; set; }
    }
}
