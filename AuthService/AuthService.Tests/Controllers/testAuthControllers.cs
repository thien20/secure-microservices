using Xunit;
using Moq;
using AuthService.Services;
using AuthService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Tests.Controllers
{
    public class testAuthControllers
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly Mock<IFailedLoginTracker> _mockFailedLoginTracker;
        // private readonly AuthController _controller;

        public testAuthControllers()
        {
            _mockUserService = new Mock<IUserService>();
            _mockTokenService = new Mock<ITokenService>();
            _mockFailedLoginTracker = new Mock<IFailedLoginTracker>();
        }

        [Fact]
        public void Login_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var mockTracker = new Mock<IFailedLoginTracker>();
            mockTracker.Setup(t => t.IsLockedOut(It.IsAny<string>())).Returns(false);

            var loginRequest = new LoginRequest
            {
                Username = "testuser",
                Password = "password123"
            };

            _mockUserService.Setup(s => s.Authenticate("testuser", "password123"))
                .Returns(new User { Username = "testuser", Role = "User", PasswordHash = "hashed_password" });

            _mockTokenService.Setup(s => s.GenerateToken("testuser", "User"))
                .Returns("mock_token");

            var controller = new AuthController(_mockUserService.Object, _mockTokenService.Object, mockTracker.Object);

            // Mock HttpContext
            var httpContext = new DefaultHttpContext();
            httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = controller.Login(loginRequest) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(result.Value);
            Assert.Contains("mock_token", result.Value.ToString());
        }

        [Fact]
        public void Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var mockTracker = new Mock<IFailedLoginTracker>();
            mockTracker.Setup(t => t.IsLockedOut(It.IsAny<string>())).Returns(false);

            var loginRequest = new LoginRequest
            {
                Username = "testuser",
                Password = "wrongppassword"
            };

            _mockUserService.Setup(s => s.Authenticate("testuser", "wrongpassword"))
                .Returns(new User { Username = "testuser", Role = "User", PasswordHash = "hashed_password" });


            var controller = new AuthController(_mockUserService.Object, _mockTokenService.Object, mockTracker.Object);

            // Mock HttpContext
            var httpContext = new DefaultHttpContext();
            httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = controller.Login(loginRequest) as UnauthorizedObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(401, result.StatusCode);
        }

        [Fact]
        public void Signup_ValidRequest_ReturnsOk()
        {
            // Arrange
            var mockTracker = new Mock<IFailedLoginTracker>();
            mockTracker.Setup(t => t.IsLockedOut(It.IsAny<string>()))
                .Returns(false);
            _mockUserService.Setup(service => service.GetUser("newuser"))
                .Returns("User not found");

            var signUpRequest = new SignUpRequest
            {
                Username = "newuser",
                Password = "password123",
                Role = "User"
            };

            // Act
            var controller = new AuthController(_mockUserService.Object, _mockTokenService.Object, mockTracker.Object);
            var result = controller.Register(signUpRequest) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(result.Value);
            Assert.Equal("User registered successfully.", result.Value.ToString());
        }
    }
}
