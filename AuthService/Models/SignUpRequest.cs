namespace AuthService.Models
{
    public class SignUpRequest
    {
        public required string Username { get; set; }
        public required string Password { get; set; }

        // Default role is "User"
        public required string Role { get; set; } = "User";
    }
}
