using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService.Models;
using Microsoft.IdentityModel.Tokens;

public class TokenService : ITokenService
{
    private readonly SymmetricSecurityKey _symmetricSecurityKey;
    private readonly string _issuer;
    private readonly string _audience;

    public TokenService(IConfiguration config)
    {
        // Initialize configuration values
        var jwtKey = config["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key", "JWT Key is not configured.");
        _symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

        _issuer = config["Jwt:Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer", "JWT Issuer is not configured.");
        _audience = config["Jwt:Audience"] ?? throw new ArgumentNullException("Jwt:Audience", "JWT Audience is not configured.");
    }

    public string GenerateToken(string username, string role)
    {
        // Claims for the token
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role)
        };

        // Signing credentials
        var creds = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        // Token generation
        var token = new JwtSecurityToken(
            _issuer,
            _audience,
            claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public User? ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            // Validate the token
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _symmetricSecurityKey,
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            // Extract claims
            var jwtToken = (JwtSecurityToken)validatedToken;
            var username = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var role = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (username == null || role == null)
            {
                return null; // Invalid token
            }

            return new User { Username = username, Role = role, PasswordHash = "hashed_password" };
        }
        catch
        {
            return null;
        }
    }
}

