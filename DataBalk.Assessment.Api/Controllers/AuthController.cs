using DataBalk.Assessment.Services.Abstractions;
using DataBalk.Assessment.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace DataBalk.Assessment.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController(IAuthService authService, IJwtTokenService jwtTokenService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        private readonly IJwtTokenService _jwtTokenService = jwtTokenService;

        /// <summary>
        /// Authenticates a user.
        /// </summary>
        /// <param name="login"></param>
        /// <returns>A generated token if user is authenticated</returns>
        /// <remarks>
        /// Sample data:
        ///
        ///     POST /Login
        ///     {
        ///        "username": "username used when user was created",
        ///        "password": "passwored used when user was created"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns the token generated.</response>
        /// <response code="401">If the user was not successfully authenticated.</response>
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginDto login)
        {
            var authenticated = await _authService.Authenticate(login.UserName, login.Password);
            
            if (!authenticated) 
                return Unauthorized();

            var token = _jwtTokenService.GenerateToken(login.UserName);

            return Ok(token);
        }
    }
}
