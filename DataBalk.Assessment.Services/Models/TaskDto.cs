namespace DataBalk.Assessment.Services.Models
{
    public class TaskDto
    {
        public long Id { get; set; }
        public long AssigneeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
    }
}
