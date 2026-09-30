using World.Simulation.Core;

namespace World.Simulation;

/// <summary>
/// Todo o estado mutável da simulação. Alterado apenas por sistemas (durante o tick) e comandos
/// (no início do tick). Tudo que estiver aqui deve entrar em <see cref="ComputeChecksum"/>.
/// </summary>
public sealed class WorldState
{
    public WorldState(ulong seed, int startYear)
    {
        Seed = seed;
        StartYear = startYear;
        Random = new WorldRandom(seed);
    }

    public ulong Seed { get; }
    public int StartYear { get; }

    /// <summary>Tick atual (dia sendo simulado), começando em 0.</summary>
    public long Tick { get; internal set; }

    public GameDate Date => new(Tick, StartYear);

    /// <summary>Único RNG da simulação. Nunca usar outra fonte de aleatoriedade.</summary>
    public WorldRandom Random { get; }

    public ulong ComputeChecksum()
    {
        var hasher = new StateHasher()
            .Add(Seed)
            .Add(StartYear)
            .Add(Tick);

        var rng = Random.GetState();
        hasher.Add(rng.S0).Add(rng.S1).Add(rng.S2).Add(rng.S3);

        return hasher.Value;
    }
}
