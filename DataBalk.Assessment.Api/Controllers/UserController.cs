using DataBalk.Assessment.Services.Abstractions;
using DataBalk.Assessment.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataBalk.Assessment.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController(IUserService userService) : ControllerBase
    {

        private readonly IUserService _userService = userService;

        /// <summary>
        /// Gets all users.
        /// </summary>        
        /// <returns>A list of all users.</returns>
        /// <response code="200">Returns an empty list if there are no users else all users.</response>        
        [HttpGet("users")]
        public async Task<ActionResult<List<UserDto>>> Users()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }

        /// <summary>
        /// Gets a user.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>User</returns>
        /// <response code="200">Returns the user if found for given id.</response>
        /// <response code="500">Internal server error if user is not found.</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById(long id)
        {
            var user = await _userService.GetUserById(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        /// <summary>
        /// Creates a user.
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns>Creates a task for a user</returns>
        /// <remarks>
        /// Sample data:
        ///
        ///     POST /Create
        ///     {
        ///        "userName": "user_name",
        ///        "email": "user.name@email.com"
        ///        "password": "P@ssW0rd"        
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns the id for successfully created user.</response>
        /// <response code="500">Internal server error if user was not successfully created.</response>
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<long>> Create(UserDto userDto)
        {
            var id = await _userService.CreateUser(userDto);
            return Ok(id);
        }

        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>No content if user is successfully deleted</returns>
        /// <response code="204">Returns no content if user was deleted successfully.</response>
        /// <response code="500">Internal server error if user was not successfully deleted.</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            await _userService.RemoveUser(id);
            return NoContent();
        }

    }
}
