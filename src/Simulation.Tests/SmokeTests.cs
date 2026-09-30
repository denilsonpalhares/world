namespace World.Simulation.Tests;

public class SmokeTests
{
    [Fact]
    public void SimulationAssemblyIsReachable()
    {
        Assert.Equal("World Simulation", SimulationInfo.Name);
    }
}
