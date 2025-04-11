using AutoMapper;
using DataBalk.Assessment.Data.Abstractions;
using DataBalk.Assessment.Data.Models;
using DataBalk.Assessment.Services;
using DataBalk.Assessment.Services.Abstractions;
using DataBalk.Assessment.Services.Models;
using DataBalk.Assessment.Utils;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System.Linq.Expressions;
using Task = System.Threading.Tasks.Task;
using TaskEntity = DataBalk.Assessment.Data.Models.Task;

namespace DataBalk.Assessment.Tests
{
    [TestFixture]
    public class TaskServiceTests
    {
        private Mock<IRepository> _repositoryMock;
        private Mock<IMapper> _mapperMock;
        private ITaskService _taskService;

        [SetUp]
        public void SetUp()
        {
            _repositoryMock = new Mock<IRepository>();
            _mapperMock = new Mock<IMapper>();
            _taskService = new TaskService(_repositoryMock.Object, _mapperMock.Object);
        }

        [Test]
        public async Task Assign_Task_Returns_Created_Task_Id()
        {

            var dueDate = new DateTime(2025, 4, 11, 12, 0, 0);

            var assignee = GetAssignee();

            var dto = new TaskDto
            {
                AssigneeId = 1,
                Title = "Testing Task",
                Description = "Creating unit tests for application.",
                DueDate = dueDate,
            };

            var taskEntity = new TaskEntity
            {
                Id = 1,
                Title = "Testing Task",
                Description = "Creating unit tests for application.",
                Assignee = assignee,
                DueDate = dueDate
            };

            _mapperMock.Setup(m => m.Map<TaskEntity>(dto)).Returns(taskEntity);

            _repositoryMock.Setup(r => r.Insert(It.IsAny<TaskEntity>())).ReturnsAsync(taskEntity);

            var result = await _taskService.AssignTask(dto);

            Assert.That(result, Is.EqualTo(1L));
        }

        [Test]
        public async Task Return_Active_Tasks()
        {
            var allTasks = GetTasks();
            
            _repositoryMock.Setup(r =>
                r.Get<TaskEntity>(
                    It.IsAny<Expression<Func<TaskEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<TaskEntity>, IIncludableQueryable<TaskEntity, object>>>()))
                .ReturnsAsync((Expression<Func<TaskEntity, bool>> predicate,
                           Func<IQueryable<TaskEntity>, IIncludableQueryable<TaskEntity, object>> include) =>
            {
                return allTasks.AsQueryable().Where(predicate).ToList();
            });

            _mapperMock.Setup(m => m.Map<IEnumerable<TaskViewModel>>(It.IsAny<IEnumerable<TaskEntity>>()))
                .Returns((IEnumerable<TaskEntity> source) =>
                {
                    return source.Select(t => new TaskViewModel
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Description = t.Description,
                        DueDate = t.DueDate,
                        Assignee = t.Assignee.UserName,
                    });
                });

            var result = await _taskService.GetAllActiveTasks();

            Assert.That(result.Count, Is.EqualTo(2));
            
        }

        [Test]
        public async Task Return_Expired_Tasks()
        {
            var allTasks = GetTasks();

            _repositoryMock.Setup(r =>
                r.Get<TaskEntity>(
                    It.IsAny<Expression<Func<TaskEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<TaskEntity>, IIncludableQueryable<TaskEntity, object>>>()))
                .ReturnsAsync((Expression<Func<TaskEntity, bool>> predicate,
                           Func<IQueryable<TaskEntity>, IIncludableQueryable<TaskEntity, object>> include) =>
                {
                    return allTasks.AsQueryable().Where(predicate).ToList();
                });

            _mapperMock.Setup(m => m.Map<IEnumerable<TaskViewModel>>(It.IsAny<IEnumerable<TaskEntity>>()))
                .Returns((IEnumerable<TaskEntity> source) =>
                {
                    return source.Select(t => new TaskViewModel
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Description = t.Description,
                        DueDate = t.DueDate,
                        Assignee = t.Assignee.UserName,
                    });
                });

            var result = await _taskService.GetAllExpiredTasks();

            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task Return_Task_That_Match_Given_Date()
        {
            var allTasks = GetTasks();

            _repositoryMock.Setup(r =>
                r.Get<TaskEntity>(
                    It.IsAny<Expression<Func<TaskEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<TaskEntity>, IIncludableQueryable<TaskEntity, object>>>()))
                .ReturnsAsync((Expression<Func<TaskEntity, bool>> predicate,
                           Func<IQueryable<TaskEntity>, IIncludableQueryable<TaskEntity, object>> include) =>
                {
                    return allTasks.AsQueryable().Where(predicate).ToList();
                });

            _mapperMock.Setup(m => m.Map<IEnumerable<TaskViewModel>>(It.IsAny<IEnumerable<TaskEntity>>()))
                .Returns((IEnumerable<TaskEntity> source) =>
                {
                    return source.Select(t => new TaskViewModel
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Description = t.Description,
                        DueDate = t.DueDate,
                        Assignee = t.Assignee.UserName,
                    });
                });

            var result = await _taskService.GetAllTasksByDate(FilterDueDate());            

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.FirstOrDefault()?.Title, Is.EqualTo("Date Filter Task And Not Active"));
        }

        [Test]
        public Task Remove_Task_Does_Not_Throw_Error()
        {
            var taskId = 1L;

            var taskEntity = new TaskEntity
            {
                Id = taskId,
                Title = "Remove Task",
                Description = "This test will remove the task",
                Assignee = GetAssignee(),
                DueDate = DateTime.UtcNow
            };

            _repositoryMock.Setup(r => r.GetById(
                    taskId,
                    It.IsAny<Func<IQueryable<TaskEntity>, IIncludableQueryable<TaskEntity, object>>>())
                ).ReturnsAsync(taskEntity);

            _repositoryMock.Setup(r => r.Delete(taskEntity))
                .Returns(Task.CompletedTask);
            
            Assert.DoesNotThrowAsync(async () => await _taskService.RemoveTask(taskId));
            return System.Threading.Tasks.Task.CompletedTask;
        }

        private User GetAssignee()
        {
            var password = "P@ssWOrd123";
            var hashSalt = EncryptionExtension.Encrypt(password);
            var encryptedPassword = hashSalt.Hash;
            var salt = hashSalt.Salt;
            return new User { Id = 1, UserName = "john_assignee", Email = "john.assignee@test.com", Salt = salt, Password = encryptedPassword };

        }

        private List<TaskEntity> GetTasks()
        {
            var today = DateTime.UtcNow;

            var assignee = GetAssignee();

            var oneTaskEntity = new TaskEntity
            {
                Id = 1,
                Title = "One Active Task",
                Description = "Creating unit tests for active tasks.",
                Assignee = assignee,
                DueDate = today.AddDays(1)
            };

            var twoTaskEntity = new TaskEntity
            {
                Id = 2,
                Title = "Two Active Task",
                Description = "Creating unit tests for active tasks.",
                Assignee = assignee,
                DueDate = today.AddDays(2)
            };

            var notActiveTaskEntity = new TaskEntity
            {
                Id = 3,
                Title = "Not Active Task",
                Description = "Creating unit tests for tasks that are not active.",
                Assignee = assignee,
                DueDate = today.AddDays(-2)
            };

            var dateFilterTaskEntity = new TaskEntity
            {
                Id = 4,
                Title = "Date Filter Task And Not Active",
                Description = "Creating unit tests for tasks that are not active.",
                Assignee = assignee,
                DueDate = FilterDueDate()
            };

            return new List<TaskEntity> { oneTaskEntity, twoTaskEntity, notActiveTaskEntity, dateFilterTaskEntity };
        }

        private DateTime FilterDueDate() 
        {
            return new DateTime(2025, 4, 05, 12, 0, 0);
        }

    }
}


