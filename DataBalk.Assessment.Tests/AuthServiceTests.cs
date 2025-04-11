using DataBalk.Assessment.Data.Abstractions;
using DataBalk.Assessment.Data.Models;
using DataBalk.Assessment.Services;
using DataBalk.Assessment.Services.Abstractions;
using DataBalk.Assessment.Utils;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System.Linq.Expressions;
using Task = System.Threading.Tasks.Task;

namespace DataBalk.Assessment.Tests
{
    [TestFixture]
    public class AuthServiceTests
    {

        private Mock<IRepository> _repositoryMock;
        private IAuthService _authService;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IRepository>();
            _authService = new AuthService(_repositoryMock.Object);
        }

        [Test]
        public async Task Authenticate_Returns_False_When_User_Is_Not_Found()
        {
            _repositoryMock.Setup(r =>
                r.Get<User>(
                    It.IsAny<Expression<Func<User, bool>>>(),
                    It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>()))
                .ReturnsAsync([]);

            var result = await _authService.Authenticate("user_not_found", "P@ssWOrd987");

            Assert.IsFalse(result);
        }

        [Test]
        public async Task Authenticate_Returns_True_When_Password_Matches()
        {            
            var password = "P@ssWOrd123";
            var hashSalt = EncryptionExtension.Encrypt(password);
            var encryptedPassword = hashSalt.Hash;
            var salt = hashSalt.Salt;

            var user = new User { UserName = "john_match", Email = "john.match@test.com", Salt = salt, Password = encryptedPassword };

            _repositoryMock.Setup(r => 
                r.Get<User>(
                    It.IsAny<Expression<Func<User, bool>>>(),
                    It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>()))
               .ReturnsAsync(new List<User> { user });

            var result = await _authService.Authenticate("john_match", password);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task Authenticate_Returns_False_When_Password_Does_Not_Match()
        {
            var password = "P@ssWOrd321";
            var hashSalt = EncryptionExtension.Encrypt(password);
            var encryptedPassword = hashSalt.Hash;
            var salt = hashSalt.Salt;

            var user = new User { UserName = "john_no_match", Email = "john.no_match@test.com", Salt = salt, Password = encryptedPassword };

            _repositoryMock.Setup(r =>
                r.Get<User>(
                    It.IsAny<Expression<Func<User, bool>>>(),
                    It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>()))
               .ReturnsAsync(new List<User> { user });

            var result = await _authService.Authenticate("john_no_match", "P@ssWOrd");

            Assert.IsFalse(result);
        }


    }
}