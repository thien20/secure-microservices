using Microsoft.AspNetCore.Mvc;
using AuthService.Models;
using AuthService.Services;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Http;


[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
        // Brute force protection constants
    private readonly IFailedLoginTracker _failedLoginTracker;
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;

    // Constructor
    public AuthController(IUserService userService, ITokenService tokenService, IFailedLoginTracker failedLoginTracker)
    {
        _userService = userService;
        _tokenService = tokenService;
        _failedLoginTracker = failedLoginTracker;

    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] SignUpRequest request)
    {
        if (_userService.GetUser(request.Username) != "User not found")
        {
            return BadRequest("User already exists.");
        }

        var user = new User
        {
            Username = request.Username,
            PasswordHash = request.Password,
            Role = request.Role
        };

        _userService.RegisterUser(user.Username, request.Password, user.Role);
        return Ok("User registered successfully.");
    }

    [HttpPost("login")]
    [EnableRateLimiting("fixed")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        // Check failed login attempts
        if (_failedLoginTracker.IsLockedOut(clientIp))
        {
            return StatusCode(429, "Too many login attempts. Try again later.");
        }

        var user = _userService.Authenticate(request.Username, request.Password);

        if (user == null)
        {
            _failedLoginTracker.TrackFailedAttempt(clientIp);
            return Unauthorized("Invalid username or password.");
        }

        _failedLoginTracker.ClearFailedAttempts(clientIp);

        var token = _tokenService.GenerateToken(user.Username, user.Role);
        return Ok(new { Token = token });
    }
}