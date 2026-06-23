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

- `GET /` serves the VehicleInsight search UI.
- `GET /api/app` returns the application name and version.
- `GET /health` returns `Healthy`.
- `GET /api/vehicles/makes` returns vehicle makes from the NHTSA Vehicle API.
- `GET /api/vehicles/makes/{makeId}/types` returns vehicle types for a make.
- `GET /api/vehicles/makes/{makeId}/models?year=2015` returns models for a make and model year.
- Swagger UI is available at `/swagger` in Development.

The static frontend is served from `src/VehicleInsight.Web/wwwroot` and is available at the root URL when the app is running.

## Docker Run Instructions

Build and run the application from the solution root:

```powershell
docker compose up --build
```

The app will be available at:

- UI: `http://localhost:8080`
- Swagger: `http://localhost:8080/swagger`

Stop the container with `Ctrl+C`, or run this from another terminal:

```powershell
docker compose down
```

## Planned Next Steps

- Add domain models for vehicle data.
- Add endpoint tests and broader application test coverage.
- Add richer error responses and resilience policies for external API calls.
