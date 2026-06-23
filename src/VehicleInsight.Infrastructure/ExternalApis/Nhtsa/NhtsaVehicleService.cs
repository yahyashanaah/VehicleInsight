using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using VehicleInsight.Application.Common.Interfaces;
using VehicleInsight.Application.DTOs;

namespace VehicleInsight.Infrastructure.ExternalApis.Nhtsa;

public sealed class NhtsaVehicleService(HttpClient httpClient) : INhtsaVehicleService
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

    public async Task<IReadOnlyList<VehicleMakeDto>> GetAllMakesAsync(CancellationToken cancellationToken)
    {
        var response = await GetFromNhtsaAsync<NhtsaResponse<NhtsaMake>>(
            "api/vehicles/getallmakes?format=json",
            cancellationToken);

        return response.Results?
            .Select(make => new VehicleMakeDto(make.MakeId, make.MakeName ?? string.Empty))
            .ToArray() ?? [];
    }

    public async Task<IReadOnlyList<VehicleTypeDto>> GetVehicleTypesForMakeAsync(int makeId, CancellationToken cancellationToken)
    {
        var response = await GetFromNhtsaAsync<NhtsaResponse<NhtsaVehicleType>>(
            $"api/vehicles/GetVehicleTypesForMakeId/{makeId}?format=json",
            cancellationToken);

        return response.Results?
            .Select(type => new VehicleTypeDto(type.VehicleTypeId, type.VehicleTypeName ?? string.Empty))
            .ToArray() ?? [];
    }

    public async Task<IReadOnlyList<VehicleModelDto>> GetModelsForMakeYearAsync(
        int makeId,
        int year,
        CancellationToken cancellationToken)
    {
        var response = await GetFromNhtsaAsync<NhtsaResponse<NhtsaModel>>(
            $"api/vehicles/GetModelsForMakeIdYear/makeId/{makeId}/modelyear/{year}?format=json",
            cancellationToken);

        return response.Results?
            .Select(model => new VehicleModelDto(
                model.MakeId,
                model.MakeName ?? string.Empty,
                model.ModelId,
                model.ModelName ?? string.Empty))
            .ToArray() ?? [];
    }

    private async Task<TResponse> GetFromNhtsaAsync<TResponse>(string requestUri, CancellationToken cancellationToken)
    {
        try
        {
            using var response = await httpClient.GetAsync(requestUri, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    $"NHTSA API request failed with status code {(int)response.StatusCode}.");
            }

            var data = await response.Content.ReadFromJsonAsync<TResponse>(JsonSerializerOptions, cancellationToken);

            return data ?? throw new InvalidOperationException("NHTSA API returned an empty response.");
        }
        catch (HttpRequestException exception)
        {
            throw new InvalidOperationException("NHTSA API request failed.", exception);
        }
        catch (TaskCanceledException exception) when (!cancellationToken.IsCancellationRequested)
        {
            throw new InvalidOperationException("NHTSA API request timed out.", exception);
        }
        catch (JsonException exception)
        {
            throw new InvalidOperationException("NHTSA API response could not be parsed.", exception);
        }
    }

    private sealed record NhtsaResponse<TResult>(
        [property: JsonPropertyName("Results")] IReadOnlyList<TResult>? Results);

    private sealed record NhtsaMake(
        [property: JsonPropertyName("Make_ID")] int MakeId,
        [property: JsonPropertyName("Make_Name")] string? MakeName);

    private sealed record NhtsaVehicleType(
        [property: JsonPropertyName("VehicleTypeId")] int VehicleTypeId,
        [property: JsonPropertyName("VehicleTypeName")] string? VehicleTypeName);

    private sealed record NhtsaModel(
        [property: JsonPropertyName("Make_ID")] int MakeId,
        [property: JsonPropertyName("Make_Name")] string? MakeName,
        [property: JsonPropertyName("Model_ID")] int ModelId,
        [property: JsonPropertyName("Model_Name")] string? ModelName);
}
