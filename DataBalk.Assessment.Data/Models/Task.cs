using DataBalk.Assessment.Data.Abstractions;

namespace DataBalk.Assessment.Data.Models
{
    
    public partial class Task : IEntity
    {
        public long Id { get; set; }
        public long AssigneeId { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required User Assignee { get; set; }
        public DateTime DueDate { get; set; }
    }
}
