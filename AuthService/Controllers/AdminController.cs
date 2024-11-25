using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace AuthService.Controllers
{
    [ApiController]
    // [Route("api/[controller]")]
    [Route("admin")]
    [Authorize(Roles = "Admin")] // Only Admins can access this controller
    public class AdminController : ControllerBase
    {
        [HttpGet("admin-only-data")]
        public IActionResult GetAdminData()
        {
            // Extract username and role from claims
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            return Ok(new {Message = $"Hello {username} with role {role}."});
        }

    }
}