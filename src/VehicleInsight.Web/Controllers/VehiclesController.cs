using Microsoft.AspNetCore.Mvc;
using VehicleInsight.Application.Common.Interfaces;
using VehicleInsight.Application.DTOs;

namespace VehicleInsight.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehiclesController(INhtsaVehicleService nhtsaVehicleService) : ControllerBase
{
    [HttpGet("makes")]
    [ProducesResponseType(typeof(IReadOnlyList<VehicleMakeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<VehicleMakeDto>>> GetMakes(CancellationToken cancellationToken)
    {
        var makes = await nhtsaVehicleService.GetAllMakesAsync(cancellationToken);

        return Ok(makes);
    }

    [HttpGet("makes/{makeId:int}/types")]
    [ProducesResponseType(typeof(IReadOnlyList<VehicleTypeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<VehicleTypeDto>>> GetVehicleTypesForMake(
        int makeId,
        CancellationToken cancellationToken)
    {
        if (makeId <= 0)
        {
            return BadRequest("makeId must be greater than 0.");
        }

        var vehicleTypes = await nhtsaVehicleService.GetVehicleTypesForMakeAsync(makeId, cancellationToken);

        return Ok(vehicleTypes);
    }

    [HttpGet("makes/{makeId:int}/models")]
    [ProducesResponseType(typeof(IReadOnlyList<VehicleModelDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<VehicleModelDto>>> GetModelsForMakeYear(
        int makeId,
        [FromQuery] int year,
        CancellationToken cancellationToken)
    {
        if (makeId <= 0)
        {
            return BadRequest("makeId must be greater than 0.");
        }

        var maximumYear = DateTime.UtcNow.Year + 1;

        if (year < 1900 || year > maximumYear)
        {
            return BadRequest($"year must be between 1900 and {maximumYear}.");
        }

        var models = await nhtsaVehicleService.GetModelsForMakeYearAsync(makeId, year, cancellationToken);

        return Ok(models);
    }
}
