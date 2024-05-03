using Microsoft.EntityFrameworkCore;
using ToDoApp.Entities;

namespace ToDoApp.DAL
{
    public class ToDoDBContext : DbContext
    {
        public ToDoDBContext(DbContextOptions<ToDoDBContext> options) : base(options)
        {
            
        }

        public DbSet<Todo> Todos { get; set; }
    }
}
