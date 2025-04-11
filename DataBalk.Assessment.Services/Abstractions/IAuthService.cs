namespace DataBalk.Assessment.Services.Abstractions
{
    public interface IAuthService
    {
        Task<bool> Authenticate(string username, string password);
    }
}
