using DataBalk.Assessment.Services.Models;

namespace DataBalk.Assessment.Services.Abstractions
{
    public interface IUserService
    {

        Task<List<UserViewModel>> GetAllUsers();

        Task<UserViewModel> GetUserById(long id);

        Task<long> CreateUser(UserDto dto);

        Task RemoveUser(long id);

    }
}
