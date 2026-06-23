FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["VehicleInsight.sln", "./"]
COPY ["src/VehicleInsight.Domain/VehicleInsight.Domain.csproj", "src/VehicleInsight.Domain/"]
COPY ["src/VehicleInsight.Application/VehicleInsight.Application.csproj", "src/VehicleInsight.Application/"]
COPY ["src/VehicleInsight.Infrastructure/VehicleInsight.Infrastructure.csproj", "src/VehicleInsight.Infrastructure/"]
COPY ["src/VehicleInsight.Web/VehicleInsight.Web.csproj", "src/VehicleInsight.Web/"]
COPY ["tests/VehicleInsight.Tests/VehicleInsight.Tests.csproj", "tests/VehicleInsight.Tests/"]

RUN dotnet restore "VehicleInsight.sln"

COPY . .

RUN dotnet publish "src/VehicleInsight.Web/VehicleInsight.Web.csproj" \
    --configuration Release \
    --output /app/publish \
    /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "VehicleInsight.Web.dll"]
