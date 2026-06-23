using VehicleInsight.Application.DTOs;

namespace VehicleInsight.Web.Endpoints;

public static class ApplicationEndpoints
{
    public static IEndpointRouteBuilder MapApplicationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/", () => Results.Ok(new ApplicationInfoDto("VehicleInsight", "1.0.0")))
            .WithName("GetApplicationInfo")
            .WithTags("Application");

        app.MapGet("/health", () => Results.Text("Healthy", "text/plain"))
            .WithName("Health")
            .WithTags("Health");

        return app;
    }
}
