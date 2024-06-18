namespace ToDoApp.DTOs
{
    public class TodoGetDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public bool Completed { get; set; }
    }
}