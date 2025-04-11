using AutoMapper;
using DataBalk.Assessment.Data.Abstractions;
using DataBalk.Assessment.Data.Models;
using DataBalk.Assessment.Services.Abstractions;
using DataBalk.Assessment.Services.Models;
using DataBalk.Assessment.Utils;
using Task = System.Threading.Tasks.Task;
using TaskEntity = DataBalk.Assessment.Data.Models.Task;

namespace DataBalk.Assessment.Services
{
    public class UserService(IRepository repository, IMapper mapper) : IUserService
    {

        private readonly IRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<long> CreateUser(UserDto dto)
        {
            var userEntity = _mapper.Map<User>(dto);
            var hashSalt = EncryptionExtension.Encrypt(userEntity.Password);
            userEntity.Password = hashSalt.Hash;
            userEntity.Salt = hashSalt.Salt;
            var created = await _repository.Insert(userEntity);
            return created.Id;
        }

        public async Task<List<UserViewModel>> GetAllUsers()
        {
            var users = await _repository.Get<User>(null);
            var allUsers = _mapper.Map<IEnumerable<UserViewModel>>(users);
            return allUsers.ToList();
        }

        public async Task<UserViewModel> GetUserById(long id)
        {
            var userQueryResult = await _repository.GetById<User>(id);
            var user = _mapper.Map<UserViewModel>(userQueryResult);
            return user;
        }

        public async Task RemoveUser(long id)
        {
            var user = await _repository.GetById<User>(id) ?? throw new Exception("User not found");

            var assignedTasks = await _repository.Get<TaskEntity>(t => t.AssigneeId == id);

            if (assignedTasks.Any())
            {
                throw new InvalidOperationException("Cannot delete a user assigned to one or more tasks.");
            }

            await _repository.Delete(user);
        }
    }
}
