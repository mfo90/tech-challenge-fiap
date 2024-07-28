using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace RegionalContactsApp.IntegrationTests
{
    public class AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public AuthControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Login_ShouldReturnToken_WhenCredentialsAreValid()
        {
            // Arrange
            var loginModel = new
            {
                Username = "admin",
                Password = "123456"
            };

            var content = new StringContent(JsonConvert.SerializeObject(loginModel), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/Auth/login", content);

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue();

            var responseContent = await response.Content.ReadAsStringAsync();
            responseContent.Should().Contain("token");
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
        {
            // Arrange
            var loginModel = new
            {
                Username = "wronguser",
                Password = "wrongpassword"
            };

            var content = new StringContent(JsonConvert.SerializeObject(loginModel), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/Auth/login", content);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }
    }
}
