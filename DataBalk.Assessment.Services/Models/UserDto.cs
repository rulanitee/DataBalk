namespace DataBalk.Assessment.Services.Models
{
    public class UserDto
    {
        public long Id { get; set; }
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
