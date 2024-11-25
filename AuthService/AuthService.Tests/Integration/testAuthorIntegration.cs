using Microsoft.AspNetCore.Mvc.Testing;
using System.Text;
using Newtonsoft.Json;
using Xunit;
using System.Net.Http.Headers;
using Moq;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;


namespace AuthService.Tests.Integration
{
    public class testAuthorIntegration: IClassFixture<WebApplicationFactory<AuthService.Startup>>
    {
        private readonly HttpClient _client;
        public testAuthorIntegration(WebApplicationFactory<AuthService.Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task AdminSignup_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var signUpRequest = new
            {
                Username = "admin",
                Password = "password123",
                Role = "Admin"
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
        public async Task Admin_GetData_ValidCredentials_ReturnsSuccess()
        {

            // Arrange: Prepare login request
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "Jwt:Key", "@hoigiadinhly123456@motngaynaodotuiselaseniorSWE" },
                    { "Jwt:Issuer", "AuthAPI" },
                    { "Jwt:Audience", "BusinessAPI" }
                })
                .Build();

            // Use the real TokenService
            var tokenService = new TokenService(config);
            var generatedToken = tokenService.GenerateToken("admin", "Admin");
            // Console.WriteLine(generatedToken);

            // Act get data with client
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", generatedToken);
            var response = await _client.GetAsync("/admin/admin-only-data");
            Assert.NotNull(response);
            response.EnsureSuccessStatusCode();

            // Assert
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.Contains("Admin", responseBody);
        }

        [Fact]
        public async Task User_GetData_ValidCredentials_ReturnsForbidden()
        {

            // Arrange: Prepare login request
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "Jwt:Key", "@hoigiadinhly123456@motngaynaodotuiselaseniorSWE" },
                    { "Jwt:Issuer", "AuthAPI" },
                    { "Jwt:Audience", "BusinessAPI" }
                })
                .Build();

            // Use the real TokenService
            var tokenService = new TokenService(config);
            var generatedToken = tokenService.GenerateToken("user", "User");
            // Console.WriteLine(generatedToken);

            // Act get data with client
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", generatedToken);
            var response = await _client.GetAsync("/admin/admin-only-data");
            Assert.NotNull(response);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}