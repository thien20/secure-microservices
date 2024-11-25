using AuthService.Models;
using System.Collections.Concurrent;


namespace AuthService.Services
{
    public class UserService : IUserService
    {
        // Thread-safe dictionary to store user data
        private readonly ConcurrentDictionary<string, User> _users = new();

        public string GetUser(string username)
        {
            // Check if the user exists
            if (_users.TryGetValue(username, out var user))
            {
                return user.Username;
            }

            return "User not found";
        }

        public void RegisterUser(string username, string password, string role)
        {
            // Check if user already exists
            if (_users.ContainsKey(username))
            {
                throw new ArgumentException("User already exists.");
            }

            /*
            To hash a password using the defaults, 
            call the BCrypt.Net.BCrypt.HashPassword(string) 
            (which will generate a random salt and hash at default cost), like this:
            */
            // Hash the password using BCrypt
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            // Add the new user to the dictionary
            var user = new User
            {
                Username = username,
                PasswordHash = hashedPassword,
                Role = role
            };

            _users[username] = user;
        }

        public User? Authenticate(string username, string password)
        {
            // Check if the user exists
            if (_users.TryGetValue(username, out var user))
            {
                // Verify the password
                if (BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                {
                    return user;
                }
            }

            // Return null if authentication fails
            return null;
        }
    }
}

