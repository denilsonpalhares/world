using World.Simulation.Commands;
using World.Simulation.Core;
using World.Simulation.Systems;

namespace World.Simulation.Tests;

/// <summary>
/// Garante a regra central do projeto: mesma seed + mesmos comandos = mesmo estado final.
/// Conforme novos sistemas forem adicionados ao pipeline padrão, este teste passa a cobri-los.
/// </summary>
public class DeterminismTests
{
    // Sistema que consome o RNG em quantidades variáveis, simulando decisões aleatórias.
    private sealed class NoisySystem : ISimulationSystem
    {
        public string Id => "noisy";
        public SystemFrequency Frequency => SystemFrequency.Daily;

        public void Update(WorldState world)
        {
            var draws = world.Random.NextInt(1, 5);
            for (var i = 0; i < draws; i++)
                world.Random.NextDouble();
        }
    }

    private sealed record DrawCommand(int Count) : ICommand
    {
        public void Execute(WorldState world)
        {
            for (var i = 0; i < Count; i++)
                world.Random.NextULong();
        }
    }

    private static ulong RunScenario(ulong seed, int years)
    {
        var pipeline = new SystemPipeline([.. SystemPipeline.CreateDefault().Systems, new NoisySystem()]);
        var simulator = new Simulator(new WorldState(seed, startYear: 1800), pipeline);

        for (var day = 0; day < years * GameDate.DaysPerYear; day++)
        {
            if (day % 17 == 0)
                simulator.Enqueue(new DrawCommand(day % 5 + 1));
            simulator.Step();
        }

        return simulator.World.ComputeChecksum();
    }

    [Fact]
    public void SameSeedAndCommandsProduceSameChecksum()
    {
        Assert.Equal(RunScenario(seed: 2026, years: 5), RunScenario(seed: 2026, years: 5));
    }

    [Fact]
    public void DifferentSeedsProduceDifferentChecksums()
    {
        Assert.NotEqual(RunScenario(seed: 1, years: 1), RunScenario(seed: 2, years: 1));
    }

    [Fact]
    public void DefaultPipelineIsDeterministic()
    {
        static ulong Run()
        {
            var simulator = new Simulator(new WorldState(seed: 99, startYear: 1800), SystemPipeline.CreateDefault());
            simulator.Run(10 * GameDate.DaysPerYear);
            return simulator.World.ComputeChecksum();
        }

        Assert.Equal(Run(), Run());
    }
}
