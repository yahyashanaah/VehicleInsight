# VehicleInsight

VehicleInsight is a .NET 8 web application for searching vehicle information from the official NHTSA VPIC API. It provides a simple responsive UI plus API endpoints for vehicle makes, vehicle types, and models by make/year.

## Live Demo

http://13.51.175.53

The deployed application is publicly available at `http://13.51.175.53`. The production EC2 instance exposes the app on public HTTP port `80`; local Docker instructions use port `8080`.

## Features

- Clean Architecture solution structure.
- ASP.NET Core Web API backend.
- Static frontend served from `wwwroot`.
- Vehicle make dropdown loaded from NHTSA data.
- Vehicle type and model search by make and manufacture year.
- Input validation for make and year.
- Swagger/OpenAPI documentation.
- Docker and Docker Compose support.
- Basic automated tests for validation and service behavior.

## Tech Stack

- .NET 8
- ASP.NET Core Web API
- C#
- System.Text.Json
- Typed HttpClient
- Swagger / Swashbuckle
- HTML, CSS, and vanilla JavaScript
- xUnit
- Docker

## Clean Architecture Overview

The solution follows Clean Architecture dependency rules:

- Domain has no dependencies and is reserved for core domain entities/value objects.
- Application defines DTOs, interfaces, and application contracts.
- Infrastructure implements external integrations, including the NHTSA VPIC API client.
- Web is the API/UI entry point and depends on Application and Infrastructure.
- Tests validate behavior without changing production dependencies.

Dependency direction:

```text
Web -> Application
Web -> Infrastructure -> Application -> Domain
```

## Project Structure

```text
VehicleInsight/
  src/
    VehicleInsight.Domain/
    VehicleInsight.Application/
      Common/Interfaces/
      DTOs/
    VehicleInsight.Infrastructure/
      ExternalApis/Nhtsa/
    VehicleInsight.Web/
      Controllers/
      Endpoints/
      wwwroot/
        css/
        js/
  tests/
    VehicleInsight.Tests/
  Dockerfile
  docker-compose.yml
  VehicleInsight.sln
```

## External API

VehicleInsight uses the official NHTSA VPIC API:

- Base URL: `https://vpic.nhtsa.dot.gov/`
- All makes: `/api/vehicles/getallmakes?format=json`
- Vehicle types by make: `/api/vehicles/GetVehicleTypesForMakeId/{makeId}?format=json`
- Models by make/year: `/api/vehicles/GetModelsForMakeIdYear/makeId/{makeId}/modelyear/{year}?format=json`

## Local Setup

Prerequisites:

- .NET 8 SDK

Restore and build:

```powershell
dotnet restore
dotnet build
```

Run the application:

```powershell
dotnet run --project src/VehicleInsight.Web
```

Open:

- UI: `http://localhost:5241`
- Swagger: `http://localhost:5241/swagger`

Run tests:

```powershell
dotnet test
```

## Docker Setup

Prerequisites:

- Docker Desktop or a compatible Docker engine

Build and run from the solution root:

```powershell
docker compose up --build
```

Open:

- UI: `http://localhost:8080`
- Swagger: `http://localhost:8080/swagger`

Stop the container:

```powershell
docker compose down
```

## Environment Variables

The app runs with sensible defaults for local development and Docker.

| Variable | Purpose | Default/Example |
| --- | --- | --- |
| `ASPNETCORE_ENVIRONMENT` | Controls runtime environment and Swagger availability. | `Development` for local/Docker demo |
| `ASPNETCORE_URLS` | Configures the container listening URL. | `http://+:8080` |

The NHTSA VPIC API base URL is configured in code as `https://vpic.nhtsa.dot.gov/`.

## API Endpoints

- `GET /` serves the VehicleInsight search UI.
- `GET /api/app` returns the application name and version.
- `GET /health` returns `Healthy`.
- `GET /api/vehicles/makes` returns all vehicle makes.
- `GET /api/vehicles/makes/{makeId}/types` returns vehicle types for a make.
- `GET /api/vehicles/makes/{makeId}/models?year={year}` returns vehicle models for a make and model year.
- `GET /swagger` opens Swagger UI in Development.

Validation rules:

- `makeId` must be greater than `0`.
- `year` must be between `1900` and the current year plus one.

## How To Use The App

1. Open the UI in the browser.
2. Wait for vehicle makes to load.
3. Select a vehicle make from the dropdown.
4. Enter a manufacture year, for example `2015`.
5. Click `Search`.
6. Review the results in the `Vehicle Types` and `Models` sections.

## Git Commit Approach

Recommended commit flow for this assignment:

- Commit foundational solution setup separately from feature work.
- Commit the NHTSA API integration as its own logical change.
- Commit frontend UI work separately from backend integration.
- Commit Docker support separately.
- Keep commit messages short and descriptive, for example:
  - `Initial clean architecture solution`
  - `Add NHTSA vehicle API integration`
  - `Add static vehicle search UI`
  - `Add Docker support`
  - `Improve assignment README`

## Future Improvements

- Add richer error responses and centralized exception handling.
- Add resilience policies for the NHTSA API, such as retries and timeouts.
- Add caching for vehicle makes to reduce repeated external API calls.
- Add more endpoint and UI integration tests.
- Add structured logging and observability.
- Add CI build/test workflow.
- Improve frontend filtering and result presentation.

## Troubleshooting

- If the local app does not start, confirm the .NET 8 SDK is installed and run `dotnet restore`.
- If Docker fails to start, confirm Docker Desktop or the Docker engine is running, then retry `docker compose up --build`.
- If `http://localhost:8080` is unavailable, check whether another process is using port `8080`.
- If vehicle data does not load, verify internet access from the app host because the service calls the external NHTSA VPIC API.
- If Swagger is unavailable on the live EC2 URL, use the local or Docker development URLs. Swagger is intended for development/demo environments.

## AWS Deployment

AWS EC2 deployment instructions are available in [docs/deployment-aws-ec2.md](docs/deployment-aws-ec2.md).

Deployment notes:

- Hosted on AWS EC2 Free Tier.
- Dockerized and running on Ubuntu EC2.
- Public HTTP port: `80`.
- Container port: `8080`.
- Public URL: http://13.51.175.53
