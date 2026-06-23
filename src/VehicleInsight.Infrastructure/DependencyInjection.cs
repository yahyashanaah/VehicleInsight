using Microsoft.Extensions.DependencyInjection;
using VehicleInsight.Application.Common.Interfaces;
using VehicleInsight.Infrastructure.ExternalApis.Nhtsa;

namespace VehicleInsight.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddHttpClient<INhtsaVehicleService, NhtsaVehicleService>(client =>
        {
            client.BaseAddress = new Uri("https://vpic.nhtsa.dot.gov/");
        });

        return services;
    }
}
