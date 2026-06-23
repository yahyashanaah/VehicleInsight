using Microsoft.AspNetCore.Mvc;
using VehicleInsight.Application.Common.Interfaces;
using VehicleInsight.Application.DTOs;
using VehicleInsight.Web.Controllers;

namespace VehicleInsight.Tests;

public class VehiclesControllerValidationTests
{
    [Fact]
    public async Task GetVehicleTypesForMake_ReturnsBadRequest_WhenMakeIdIsInvalid()
    {
        var controller = new VehiclesController(new StubNhtsaVehicleService());

        var result = await controller.GetVehicleTypesForMake(0, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetModelsForMakeYear_ReturnsBadRequest_WhenYearIsOutOfRange()
    {
        var controller = new VehiclesController(new StubNhtsaVehicleService());

        var result = await controller.GetModelsForMakeYear(1, 1899, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    private sealed class StubNhtsaVehicleService : INhtsaVehicleService
    {
        public Task<IReadOnlyList<VehicleMakeDto>> GetAllMakesAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyList<VehicleMakeDto>>([]);
        }

        public Task<IReadOnlyList<VehicleTypeDto>> GetVehicleTypesForMakeAsync(
            int makeId,
            CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyList<VehicleTypeDto>>([]);
        }

        public Task<IReadOnlyList<VehicleModelDto>> GetModelsForMakeYearAsync(
            int makeId,
            int year,
            CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyList<VehicleModelDto>>([]);
        }
    }
}
