using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Artemis_Banking_Pro;

namespace ABP.Integration.Test.Tests
{
    public class CajeroControllerIntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public CajeroControllerIntegrationTests(WebApplicationFactory<Startup> factory)
        {
            // Create client with default test server
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Dashboard_ReturnsSuccessAndView()
        {
            // Act
            var response = await _client.GetAsync("/Cajero/Dashboard");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Dashboard", content); // simple check that view rendered
        }
    }
}
