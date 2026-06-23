using System.Net;
using VehicleInsight.Infrastructure.ExternalApis.Nhtsa;

namespace VehicleInsight.Tests;

public class NhtsaVehicleServiceTests
{
    [Fact]
    public async Task GetAllMakesAsync_ReturnsEmptyList_WhenNhtsaReturnsNoResults()
    {
        using var httpClient = new HttpClient(new StubHttpMessageHandler("""{"Results":[]}"""))
        {
            BaseAddress = new Uri("https://vpic.nhtsa.dot.gov/")
        };
        var service = new NhtsaVehicleService(httpClient);

        var result = await service.GetAllMakesAsync(CancellationToken.None);

        Assert.Empty(result);
    }

    private sealed class StubHttpMessageHandler(string content) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(content)
            };

            return Task.FromResult(response);
        }
    }
}
