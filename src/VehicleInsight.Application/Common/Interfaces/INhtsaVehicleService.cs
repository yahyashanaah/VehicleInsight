using VehicleInsight.Application.DTOs;

namespace VehicleInsight.Application.Common.Interfaces;

public interface INhtsaVehicleService
{
    Task<IReadOnlyList<VehicleMakeDto>> GetAllMakesAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<VehicleTypeDto>> GetVehicleTypesForMakeAsync(int makeId, CancellationToken cancellationToken);

    Task<IReadOnlyList<VehicleModelDto>> GetModelsForMakeYearAsync(int makeId, int year, CancellationToken cancellationToken);
}
