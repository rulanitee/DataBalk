using Microsoft.AspNetCore.Mvc;
using DataBalk.Assessment.Services.Models;
using DataBalk.Assessment.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;

namespace DataBalk.Assessment.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class TaskController(ITaskService taskService) : ControllerBase
    {

        private readonly ITaskService _taskService = taskService;

        /// <summary>
        /// Creates a task for user.
        /// </summary>
        /// <param name="taskDto"></param>
        /// <returns>Creates a task for a user</returns>
        /// <remarks>
        /// Sample data:
        ///
        ///     POST /Create
        ///     {
        ///        "assigneeId": 1,
        ///        "title": "StandUp"
        ///        "description": "Daily standup to catch up with developers on the progress of tasks"
        ///        "dueDate": "2025-04-11T12:00:00"        
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns the id for successfully created tasks.</response>
        /// <response code="500">Internal server error if task was not successfully created.</response>
        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] TaskDto taskDto)
        {
            var id = await _taskService.AssignTask(taskDto);
            return Ok(id);
        }

        /// <summary>
        /// Deletes a task.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Deletes a task with the id that has been provided.</returns>
        /// <response code="204">Returns no content if task was deleted successfully.</response>
        /// <response code="500">Internal server error if task was not successfully deleted.</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            await _taskService.RemoveTask(id);
            return NoContent();
        }

        /// <summary>
        /// Gets all expired tasks.
        /// </summary>        
        /// <returns>A list of all expired tasks.</returns>
        /// <response code="200">Returns an empty list if there are no expired tasks else a list of expired tasks.</response>        
        [HttpGet("expired")]
        public async Task<ActionResult<List<TaskViewModel>>> GetExpiredTasks()
        {
            var tasks = await _taskService.GetAllExpiredTasks();
            return Ok(tasks);
        }

        /// <summary>
        /// Gets all active tasks.
        /// </summary>       
        /// <returns>A list of all active tasks</returns>
        /// <response code="200">Returns an empty list if there are no active tasks else a list of active tasks.</response>         
        [HttpGet("active")]
        public async Task<ActionResult<List<TaskViewModel>>> GetActiveTasks()
        {
            var tasks = await _taskService.GetAllActiveTasks();
            return Ok(tasks);
        }

        /// <summary>
        /// Gets tasks filtered by date.
        /// </summary>
        /// <param name="date"></param>
        /// <returns>A list of tasks filtered by the given date.</returns>
        /// <remarks>
        /// Sample data:
        /// GET /byDate?date='2025-04-11T12:00:00'
        /// </remarks>
        /// <response code="200">Returns an empty list if there are no matched tasks for given date else a list of matched tasks.</response>        
        [HttpGet("byDate")]
        public async Task<ActionResult<List<TaskViewModel>>> GetTasksByDate([FromQuery] DateTime date)
        {
            var tasks = await _taskService.GetAllTasksByDate(date);
            return Ok(tasks);
        }

    }
}
