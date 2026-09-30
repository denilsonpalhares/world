using World.Simulation.Commands;
using World.Simulation.Systems;

namespace World.Simulation;

/// <summary>
/// Avança a simulação. Cada <see cref="Step"/> simula um dia:
/// 1. executa os comandos pendentes; 2. roda os sistemas devidos; 3. avança o tick.
/// </summary>
public sealed class Simulator
{
    private readonly SystemPipeline _pipeline;

    public Simulator(WorldState world, SystemPipeline pipeline)
    {
        World = world;
        _pipeline = pipeline;
    }

    public WorldState World { get; }

    public CommandQueue Commands { get; } = new();

    public void Enqueue(ICommand command) => Commands.Enqueue(command);

    public void Step()
    {
        foreach (var command in Commands.Drain())
            command.Execute(World);

        _pipeline.Run(World);

        World.Tick++;
    }

    public void Run(long ticks)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(ticks);
        for (var i = 0L; i < ticks; i++)
            Step();
    }
}
