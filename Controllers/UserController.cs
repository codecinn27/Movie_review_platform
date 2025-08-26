using Microsoft.AspNetCore.Mvc;
using MovieAPI.Interface;
using MovieAPI.Contracts;
using Microsoft.AspNetCore.Authorization;

namespace MovieAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userServices;

        public UserController(IUserService userService)
        {
            _userServices = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // await _userServices.RegisterAsync(request, isAdmin: true); //role set to admin
                await _userServices.RegisterAsync(request); //role set to user
                return Ok(new { message = "User Register successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while registering user", error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
        {
            var response = await _userServices.LoginAsync(request);
            return Ok(response);
        }

        // GET: api/Users/{id}
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _userServices.FindAsync(id);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(user);
        }

    }
}