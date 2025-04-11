using DataBalk.Assessment.Api.Controllers;
using DataBalk.Assessment.Services.Abstractions;
using DataBalk.Assessment.Services.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DataBalk.Assessment.Tests.ControllerTests
{
    [TestFixture]
    public class TaskControllerTests
    {
        private Mock<ITaskService> _taskServiceMock;
        private TaskController _controller;

        [SetUp]
        public void SetUp()
        {
            _taskServiceMock = new Mock<ITaskService>();
            _controller = new TaskController(_taskServiceMock.Object);
        }

        [Test]
        public async Task Create_Returns_Ok_With_Task_Id()
        {
            var dto = new TaskDto
            {
                AssigneeId = 1,
                Title = "Controller Task",
                Description = "Controller Task Testing",
                DueDate = DateTime.UtcNow,
            };

            _taskServiceMock.Setup(s => s.AssignTask(dto)).ReturnsAsync(31);

            var result = await _controller.Create(dto);

            Assert.IsInstanceOf<OkObjectResult>(result.Result);

            var okResult = result.Result as OkObjectResult;

            Assert.That(okResult?.Value, Is.EqualTo(31));
        }

        [Test]
        public async Task Delete_Task_Return_No_Content()
        {            
            long taskId = 1;
            _taskServiceMock.Setup(s => s.RemoveTask(taskId)).Returns(Task.CompletedTask);           
            var result = await _controller.Delete(taskId);           
            Assert.That(result, Is.InstanceOf<NoContentResult>());
            _taskServiceMock.Verify(s => s.RemoveTask(taskId), Times.Once);
        }

        [Test]
        public async Task Active_Returns_All_Active_Tasks()
        {
            
            var tasks = new List<TaskViewModel>
            {
                new TaskViewModel 
                {
                    Id = 1L,
                    Assignee = "john_controller", 
                    Title = "First Task", 
                    Description = "John Controller Test",
                    DueDate = DateTime.UtcNow
                },
                new TaskViewModel
                { 
                    Id = 2L, 
                    Assignee = "jane_controller", 
                    Title = "Second Task", 
                    Description = "Jane Controller Test", 
                    DueDate = DateTime.UtcNow 
                }
            };

            _taskServiceMock.Setup(s => s.GetAllActiveTasks()).ReturnsAsync(tasks);            
            var result = await _controller.GetActiveTasks();
            var okResult = result.Result as OkObjectResult;         
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            
        }

        [Test]
        public async Task Expired_Returns_All_Expired_Tasks()
        {            
    
            var tasks = new List<TaskViewModel>
            {
                new TaskViewModel
                {
                    Id = 1L,
                    Assignee = "john_controller",
                    Title = "First Task",
                    Description = "John Controller Test",
                    DueDate = DateTime.UtcNow
                },
                new TaskViewModel
                {
                    Id = 2L,
                    Assignee = "jane_controller",
                    Title = "Second Task",
                    Description = "Jane Controller Test",
                    DueDate = DateTime.UtcNow
                }
            };    

            _taskServiceMock.Setup(s => s.GetAllExpiredTasks()).ReturnsAsync(tasks);            
            var result = await _controller.GetExpiredTasks();
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task Date_Filter_Returns_Matched_Tasks()
        {
            
            var date = new DateTime(2025, 04, 11);

            var tasks = new List<TaskViewModel>
            {
                new TaskViewModel
                {
                    Id = 61L,
                    Assignee = "john_controller",
                    Title = "First Task",
                    Description = "John Controller Test",
                    DueDate = date
                }
            };

            _taskServiceMock.Setup(s => s.GetAllTasksByDate(date)).ReturnsAsync(tasks);
          
            var result = await _controller.GetTasksByDate(date);
            var okResult = result.Result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
        }
    }
}
