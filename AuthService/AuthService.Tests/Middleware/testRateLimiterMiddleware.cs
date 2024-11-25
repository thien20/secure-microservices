using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace AuthService.Tests.Middleware
{
    public class testRateLimiterMiddleware : IClassFixture<WebApplicationFactory<AuthService.Startup>>
    {
        private readonly HttpClient _client;

        public testRateLimiterMiddleware(WebApplicationFactory<AuthService.Startup> factory)
        {
            // Configure the test server and client
            _client = factory.WithWebHostBuilder(builder =>
            {
                // Optional: Additional configuration for test environment
            }).CreateClient();
        }

        [Fact]
        public async Task Login_ExceedRateLimit_ReturnsTooManyRequests()
        {
            // Arrange
            var loginRequest = new
            {
                Username = "testuser",
                Password = "wrongpassword"
            };
            var content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(loginRequest),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            HttpResponseMessage? lastResponse = null;

            // Act: Exceed rate limit
            for (int i = 0; i < 15; i++)
            {
                lastResponse = await _client.PostAsync("/auth/login", content);
            }

            // Assert
            Assert.NotNull(lastResponse);
            Assert.Equal((int)HttpStatusCode.TooManyRequests, (int)lastResponse.StatusCode);
        }

    }
}
