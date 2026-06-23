# VehicleInsight

VehicleInsight is a .NET 8 ASP.NET Core Web API built with Clean Architecture. The initial application exposes a small API surface for application metadata and health checks, with room to add vehicle data integrations later.

## Architecture Structure

- `src/VehicleInsight.Domain` contains core domain entities and value objects. It has no project references.
- `src/VehicleInsight.Application` contains DTOs, interfaces, and future use cases/services. It references Domain.
- `src/VehicleInsight.Infrastructure` contains future external API and persistence implementations. It references Application.
- `src/VehicleInsight.Web` is the ASP.NET Core API entry point. It references Application and Infrastructure.
- `tests/VehicleInsight.Tests` contains automated tests for the solution.

## Local Run Instructions

Restore and build the solution:

```powershell
dotnet restore
dotnet build
```

Run the web API:

```powershell
dotnet run --project src/VehicleInsight.Web
```

Available endpoints:

- `GET /` returns the application name and version.
- `GET /health` returns `Healthy`.
- Swagger UI is available at `/swagger` in Development.

## Planned Next Steps

- Add domain models for vehicle data.
- Add application use cases and service interfaces.
- Add infrastructure implementations for external vehicle APIs.
- Add endpoint tests and broader application test coverage.
