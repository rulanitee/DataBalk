using DataBalk.Assessment.Data.Abstractions;

namespace DataBalk.Assessment.Data.Models
{    
    public partial class User : IEntity
    {
        public long Id { get; set; }
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required byte[] Salt { get; set; }

        public virtual ICollection<Task> Tasks { get; set; } = [];   
    }
}
