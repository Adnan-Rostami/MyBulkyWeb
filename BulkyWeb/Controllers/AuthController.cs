using BulkyWeb.Models.DTO.RefreshToken;
using BulkyWeb.Models.DTO.User;
using BulkyWeb.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace BulkyWeb.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO model)
        {
            var result = await _authService.RegisterAsync(model);

            return Ok("User Created");
        }

        //https://localhost:7210/auth/login
        [AllowAnonymous]
        [EnableRateLimiting("login")]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            var result = await _authService.LoginAsync(model);

            if (result == null)
                return Unauthorized("Email or password is wrong");

            return Ok(result);
        }

        //https://localhost:7210/auth/me
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();
            var userInfo = await _authService.GetUserInfoAsync(userId);

            if (userInfo == null)
                return NotFound("User not found");

            return Ok(userInfo);
        }










        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDTO model)
        {
            var result = await _authService.RefreshTokenAsync(model.RefreshToken);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }


    }
}