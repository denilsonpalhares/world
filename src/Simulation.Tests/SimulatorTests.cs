using World.Simulation.Commands;
using World.Simulation.Core;
using World.Simulation.Systems;

namespace World.Simulation.Tests;

public class SimulatorTests
{
    private sealed class RecordingSystem(string id, SystemFrequency frequency, List<string> log) : ISimulationSystem
    {
        public string Id => id;
        public SystemFrequency Frequency => frequency;
        public void Update(WorldState world) => log.Add($"{world.Tick}:{id}");
    }

    private sealed record RecordingCommand(string Name, List<string> Log) : ICommand
    {
        public void Execute(WorldState world) => Log.Add($"{world.Tick}:cmd:{Name}");
    }

    private static Simulator CreateSimulator(params ISimulationSystem[] systems) =>
        new(new WorldState(seed: 1, startYear: 1800), new SystemPipeline(systems));

    [Fact]
    public void StepAdvancesOneTick()
    {
        var simulator = CreateSimulator();

        simulator.Step();
        simulator.Step();

        Assert.Equal(2, simulator.World.Tick);
        Assert.Equal("1800-01-03", simulator.World.Date.ToString());
    }

    [Fact]
    public void SystemsRunInPipelineOrder()
    {
        var log = new List<string>();
        var simulator = CreateSimulator(
            new RecordingSystem("b", SystemFrequency.Daily, log),
            new RecordingSystem("a", SystemFrequency.Daily, log),
            new RecordingSystem("c", SystemFrequency.Daily, log));

        simulator.Step();

        Assert.Equal(["0:b", "0:a", "0:c"], log);
    }

    [Fact]
    public void SystemsRunAtTheirFrequency()
    {
        var log = new List<string>();
        var simulator = CreateSimulator(
            new RecordingSystem("daily", SystemFrequency.Daily, log),
            new RecordingSystem("monthly", SystemFrequency.Monthly, log),
            new RecordingSystem("yearly", SystemFrequency.Yearly, log));

        simulator.Run(2 * GameDate.DaysPerYear);

        Assert.Equal(2 * GameDate.DaysPerYear, log.Count(e => e.EndsWith(":daily")));
        Assert.Equal(2 * GameDate.MonthsPerYear, log.Count(e => e.EndsWith(":monthly")));
        Assert.Equal(["0:yearly", "360:yearly"], log.Where(e => e.EndsWith(":yearly")));
    }

    [Fact]
    public void CommandsRunAtStartOfNextTickInArrivalOrder()
    {
        var log = new List<string>();
        var simulator = CreateSimulator(new RecordingSystem("sys", SystemFrequency.Daily, log));

        simulator.Step();
        simulator.Enqueue(new RecordingCommand("first", log));
        simulator.Enqueue(new RecordingCommand("second", log));
        simulator.Step();
        simulator.Step();

        Assert.Equal(["0:sys", "1:cmd:first", "1:cmd:second", "1:sys", "2:sys"], log);
        Assert.Equal(0, simulator.Commands.Count);
    }

    [Fact]
    public void DuplicateSystemIdsAreRejected()
    {
        var log = new List<string>();

        Assert.Throws<ArgumentException>(() => new SystemPipeline([
            new RecordingSystem("same", SystemFrequency.Daily, log),
            new RecordingSystem("same", SystemFrequency.Monthly, log),
        ]));
    }
}
