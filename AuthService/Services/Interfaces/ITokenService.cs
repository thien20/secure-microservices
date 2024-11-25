using AuthService.Models;

public interface ITokenService
{
    string GenerateToken(string username, string role);
    User? ValidateToken(string token);
}