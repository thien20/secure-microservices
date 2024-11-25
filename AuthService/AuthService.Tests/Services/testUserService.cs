using Xunit;
using AuthService.Services;

namespace AuthService.Tests.Services
{
    public class testUserService
    {
        private readonly UserService _service;

        public testUserService()
        {
            _service = new UserService();
        }

        [Fact]
        public void Authenticate_ValidUser_ReturnsUser()
        {
            // Arrange
            _service.RegisterUser("testuser", "password123", "User");

            // Act
            var user = _service.Authenticate("testuser", "password123");

            // Assert
            Assert.NotNull(user);
            Assert.Equal("testuser", user.Username);
            Assert.Equal("User", user.Role);
        }

        [Fact]
        public void Authenticate_InvalidUser_ReturnsNull()
        {
            // Arrange
            _service.RegisterUser("testuser", "password123", "User");

            // Act
            var user = _service.Authenticate("wronguser", "password123");

            // Assert
            Assert.Null(user);
        }
    }
}
