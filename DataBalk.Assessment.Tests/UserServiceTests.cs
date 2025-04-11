using AutoMapper;
using DataBalk.Assessment.Data.Abstractions;
using DataBalk.Assessment.Services.Abstractions;
using DataBalk.Assessment.Services;
using Moq;
using DataBalk.Assessment.Services.Models;
using DataBalk.Assessment.Data.Models;
using DataBalk.Assessment.Utils;
using Task = System.Threading.Tasks.Task;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using TaskEntity = DataBalk.Assessment.Data.Models.Task;

namespace DataBalk.Assessment.Tests
{

    [TestFixture]
    public class UserServiceTests
    {

        private Mock<IRepository> _repositoryMock;
        private Mock<IMapper> _mapperMock;
        private IUserService _userService;

        [SetUp]
        public void SetUp()
        {
            _repositoryMock = new Mock<IRepository>();
            _mapperMock = new Mock<IMapper>();
            _userService = new UserService(_repositoryMock.Object, _mapperMock.Object);
        }

        [Test]
        public async Task Create_User_Returns_Created_User_Id()
        {
            var dto = new UserDto
            {
                UserName = "create_john",
                Email = "create.john@test.com",
                Password = "P@ssW0rdTest"
            };

            var password = "P@ssW0rdTest";
            var hashSalt = EncryptionExtension.Encrypt(password);
            var encryptedPassword = hashSalt.Hash;
            var salt = hashSalt.Salt;
            var user = new User { Id = 1L, UserName = "create_john", Email = "create.john@test.com", Salt = salt, Password = encryptedPassword };

            _mapperMock.Setup(m => m.Map<User>(dto)).Returns(user);

            _repositoryMock.Setup(r => r.Insert(It.IsAny<User>())).ReturnsAsync(user);

            var result = await _userService.CreateUser(dto);

            Assert.That(result, Is.EqualTo(1L));
        }

        [Test]
        public async Task Return_All_Users()
        {
            var password = "P@ssWOrd123";
            var hashSalt = EncryptionExtension.Encrypt(password);
            var encryptedPassword = hashSalt.Hash;
            var salt = hashSalt.Salt;

            var firstUser = new User { Id = 1L, UserName = "first_user", Email = "first.user@test.com", Salt = salt, Password = encryptedPassword };

            var secondUser = new User { Id = 2L, UserName = "second_user", Email = "second.user@test.com", Salt = salt, Password = encryptedPassword };

            var allUsers = new List<User> { firstUser, secondUser };

            _repositoryMock.Setup(r =>
            r.Get<User>(
                It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>()))
            .ReturnsAsync((Expression<Func<User, bool>>? predicate,
            Func<IQueryable<User>, IIncludableQueryable<User, object>>? include) =>
            {
                var query = allUsers.AsQueryable();

                if (predicate != null)
                    query = query.Where(predicate);
                     
                return query.ToList();
            });

            _mapperMock.Setup(m => m.Map<IEnumerable<UserViewModel>>(It.IsAny<IEnumerable<User>>()))
                .Returns((IEnumerable<User> source) =>
                {
                    return source.Select(u => new UserViewModel
                    {
                        Id = u.Id,
                        UserName = u.UserName,
                        Email = u.Email
                    });
                });

            var result = await _userService.GetAllUsers();

            Assert.That(result.Count, Is.EqualTo(2));

        }

        [Test]
        public async Task Return_User_That_Match_User_Id()
        {

            var password = "P@ssWOrd123";
            var hashSalt = EncryptionExtension.Encrypt(password);
            var encryptedPassword = hashSalt.Hash;
            var salt = hashSalt.Salt;

            var firstUser = new User { Id = 1L, UserName = "first_user", Email = "first.user@test.com", Salt = salt, Password = encryptedPassword };

            var secondUser = new User { Id = 2L, UserName = "second_user", Email = "second.user@test.com", Salt = salt, Password = encryptedPassword };

            var allUsers = new List<User> { firstUser, secondUser };

            _repositoryMock.Setup(r => r.GetById<User>(
                It.IsAny<long>(),
                It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>?>()))
            .ReturnsAsync((long id, Func<IQueryable<User>, IIncludableQueryable<User, object>>? include) =>
            {
                return allUsers.FirstOrDefault(u => u.Id == id)!;
            });

            _mapperMock.Setup(m => m.Map<UserViewModel>(It.IsAny<User>()))
            .Returns((User u) => new UserViewModel
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email
            });

            var result = await _userService.GetUserById(2L);

            Assert.That(result?.Email, Is.EqualTo("second.user@test.com"));
        }

        [Test]
        public Task Remove_User_Not_Assigned_To_Task()
        {
            var password = "P@ssWOrd123";
            var hashSalt = EncryptionExtension.Encrypt(password);
            var encryptedPassword = hashSalt.Hash;
            var salt = hashSalt.Salt;

            var removeUser = new User { Id = 1L, UserName = "remove_user", Email = "remove.user@test.com", Salt = salt, Password = encryptedPassword };

            _repositoryMock.Setup(r => r.GetById(
                    1L,
                    It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>())
                ).ReturnsAsync(removeUser);

            _repositoryMock.Setup(r => r.Delete(removeUser))
                .Returns(Task.CompletedTask);

            Assert.DoesNotThrowAsync(async () => await _userService.RemoveUser(1L));
            return System.Threading.Tasks.Task.CompletedTask;
        }

        [Test]
        public Task Remove_User_Assigned_To_Task_Fails()
        {
            var password = "P@ssWOrd123";
            var hashSalt = EncryptionExtension.Encrypt(password);
            var encryptedPassword = hashSalt.Hash;
            var salt = hashSalt.Salt;

            var userId = 1L;

            var user = new User
            {
                Id = userId,
                UserName = "assigned_user",
                Email = "assigned.user@test.com",
                Password = encryptedPassword,
                Salt = salt
            };

            var task = new TaskEntity
            {
                Id = 1,
                Title = "Assigned Task",
                Description = "Assigned Task To User",
                AssigneeId = userId,
                Assignee = user,
                DueDate = DateTime.UtcNow.AddDays(1)
            };
            
            _repositoryMock.Setup(r => r.GetById(userId,
                It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>())
            ).ReturnsAsync(user);

            _repositoryMock.Setup(r =>
             r.Get(
                It.IsAny<Expression<Func<TaskEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TaskEntity>, IIncludableQueryable<TaskEntity, object>>>()))
            .ReturnsAsync((Expression<Func<TaskEntity, bool>> predicate,
             Func<IQueryable<TaskEntity>, IIncludableQueryable<TaskEntity, object>> include) =>
            {
                return new List<TaskEntity> { task };
            }); 
            
            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await _userService.RemoveUser(userId));
            Assert.That(ex.Message, Is.EqualTo("Cannot delete a user assigned to one or more tasks."));
            return System.Threading.Tasks.Task.CompletedTask;
        }
    }
}
