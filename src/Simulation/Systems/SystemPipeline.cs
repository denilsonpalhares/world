using World.Simulation.Core;

namespace World.Simulation.Systems;

/// <summary>Lista ordenada de sistemas executada a cada tick.</summary>
public sealed class SystemPipeline
{
    private readonly ISimulationSystem[] _systems;

    public SystemPipeline(IEnumerable<ISimulationSystem> systems)
    {
        _systems = systems.ToArray();

        var duplicate = _systems.GroupBy(s => s.Id).FirstOrDefault(g => g.Count() > 1);
        if (duplicate is not null)
            throw new ArgumentException($"Sistema duplicado no pipeline: '{duplicate.Key}'.", nameof(systems));
    }

    public IReadOnlyList<ISimulationSystem> Systems => _systems;

    /// <summary>
    /// Pipeline oficial do jogo. A ordem é fixa e faz parte do determinismo:
    /// Demografia → Educação → Produção → Mercado/Comércio → Governo/Orçamento → Felicidade/Estabilidade
    /// → Migração → Diplomacia → Guerra. Os sistemas são adicionados aqui conforme forem implementados.
    /// </summary>
    public static SystemPipeline CreateDefault() => new([]);

    public void Run(WorldState world)
    {
        var date = world.Date;
        foreach (var system in _systems)
        {
            if (IsDue(system.Frequency, date))
                system.Update(world);
        }
    }

    private static bool IsDue(SystemFrequency frequency, GameDate date) => frequency switch
    {
        SystemFrequency.Daily => true,
        SystemFrequency.Monthly => date.IsFirstDayOfMonth,
        SystemFrequency.Yearly => date.IsFirstDayOfYear,
        _ => throw new ArgumentOutOfRangeException(nameof(frequency), frequency, null),
    };
}
