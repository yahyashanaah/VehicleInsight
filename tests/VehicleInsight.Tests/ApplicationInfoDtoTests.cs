using VehicleInsight.Application.DTOs;

namespace VehicleInsight.Tests;

public class ApplicationInfoDtoTests
{
    [Fact]
    public void CreatesApplicationInfo()
    {
        var info = new ApplicationInfoDto("VehicleInsight", "1.0.0");

        Assert.Equal("VehicleInsight", info.Name);
        Assert.Equal("1.0.0", info.Version);
    }
}
