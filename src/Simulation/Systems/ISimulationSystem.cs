namespace World.Simulation.Systems;

public enum SystemFrequency
{
    /// <summary>Roda todo tick.</summary>
    Daily,

    /// <summary>Roda no primeiro dia de cada mês.</summary>
    Monthly,

    /// <summary>Roda no primeiro dia de cada ano.</summary>
    Yearly,
}

/// <summary>
/// Um aspecto da simulação (demografia, produção, migração...). Cada sistema tem responsabilidade única
/// e roda na ordem definida em <see cref="SystemPipeline.CreateDefault"/>.
/// </summary>
public interface ISimulationSystem
{
    string Id { get; }

    SystemFrequency Frequency { get; }

    void Update(WorldState world);
}
