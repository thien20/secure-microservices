using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.RateLimiting;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("user")]
    [Authorize] 
    public class UserController : ControllerBase
    {
        [HttpGet("data")]
        [EnableRateLimiting("fixed")]
        public IActionResult GetUserData()
        {
            // Extract username and role from claims
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(new { Message = $"Hello {username} - you are a/an {role}." });
        }
    }
}
