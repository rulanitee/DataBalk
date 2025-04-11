using DataBalk.Assessment.Data.Abstractions;
using DataBalk.Assessment.Data.Models;
using DataBalk.Assessment.Services.Abstractions;
using DataBalk.Assessment.Utils;

namespace DataBalk.Assessment.Services
{
    public class AuthService(IRepository repository) : IAuthService
    {

        private readonly IRepository _repository = repository;

        public async Task<bool> Authenticate(string username, string password)
        {
            var userResult = await _repository.Get<User>(x => x.UserName.Equals(username));

            if (!userResult.Any())
                return false;

            var user = userResult.FirstOrDefault();

            return EncryptionExtension.VerifyEncryption(password, user.Salt, user.Password);

        }
    }
}

