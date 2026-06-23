using System.Net;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
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
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var service = CreateService(httpClient, memoryCache);

        var result = await service.GetAllMakesAsync(CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllMakesAsync_ReturnsMakesFromCache_WhenCacheIsPopulated()
    {
        var handler = new StubHttpMessageHandler("""{"Results":[{"Make_ID":1,"Make_Name":"First"}]}""");
        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://vpic.nhtsa.dot.gov/")
        };
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var service = CreateService(httpClient, memoryCache);

        await service.GetAllMakesAsync(CancellationToken.None);
        var result = await service.GetAllMakesAsync(CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("First", result[0].MakeName);
        Assert.Equal(1, handler.RequestCount);
    }

    private static NhtsaVehicleService CreateService(HttpClient httpClient, IMemoryCache memoryCache)
    {
        return new NhtsaVehicleService(
            httpClient,
            memoryCache,
            NullLogger<NhtsaVehicleService>.Instance);
    }

    private sealed class StubHttpMessageHandler(string content) : HttpMessageHandler
    {
        public int RequestCount { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            RequestCount++;

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(content)
            };

            return Task.FromResult(response);
        }
    }
}
