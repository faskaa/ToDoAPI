namespace ToDoApp.Entities
{
    public class Todo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public bool IsDeleted { get; set; }
        public bool Completed { get; set; }


        public string CustomUserId { get; set; }
        public CustomUser CustomUser { get; set; }
    }
}