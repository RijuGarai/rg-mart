using IdentityService.Dtos;
using IdentityService.Services;
using IdentityService.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IdentityService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IUserService userService, JwtService jwtService) : ControllerBase
    {
        private readonly IUserService _userService = userService;
        private readonly JwtService _jwtService = jwtService;

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserRequest request)
        {
            var user = await _userService.RegisterAsync(request);
            return Ok(new { user.Id, user.Email });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = await _userService.LoginAsync(request);
            if (user == null)
                return Unauthorized();

            var token = _jwtService.GenerateToken(user);

            return Ok(new { Token = token });
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            return Ok("You are authenticated");
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> Profile()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();
            var user = await _userService.GetUserAsync(Guid.Parse(userId));
            if (user == null)
                return NotFound();
            return Ok(new { user.Id, user.Email, user.CreatedAt });
        }
    }
}
