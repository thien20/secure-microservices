using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace AuthService.Tests.Integration
{
    public class testAuthControllersIntegration : IClassFixture<WebApplicationFactory<AuthService.Startup>>
    {
        private readonly HttpClient _client;

        public testAuthControllersIntegration(WebApplicationFactory<AuthService.Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Signup_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var signUpRequest = new
            {
                Username = "newuser",
                Password = "password123",
                Role = "User"
            };
            var content = new StringContent(JsonConvert.SerializeObject(signUpRequest), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/auth/register", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.Contains("User registered successfully.", responseBody);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var loginRequest = new
            {
                Username = "newuser",
                Password = "password123"
            };
            var content = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/auth/login", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.Contains("token", responseBody);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginRequest = new
            {
                Username = "newuser",
                Password = "wrongpassword"
            };
            var content = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/auth/login", content);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
