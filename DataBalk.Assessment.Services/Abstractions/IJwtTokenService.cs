namespace DataBalk.Assessment.Services.Abstractions
{
    public interface IJwtTokenService
    {
        string GenerateToken(string username);
    }
}
