using System.Numerics;

namespace World.Simulation.Core;

/// <summary>
/// Gerador pseudoaleatório determinístico (xoshiro256**) usado por toda a simulação.
/// Diferente de <see cref="System.Random"/>, o estado interno é exposto para save/load e o algoritmo
/// é fixo, garantindo o mesmo resultado em qualquer máquina e versão do .NET.
/// </summary>
public sealed class WorldRandom
{
    private ulong _s0, _s1, _s2, _s3;

    public WorldRandom(ulong seed)
    {
        var x = seed;
        _s0 = SplitMix64(ref x);
        _s1 = SplitMix64(ref x);
        _s2 = SplitMix64(ref x);
        _s3 = SplitMix64(ref x);
    }

    private WorldRandom(RandomState state)
    {
        if ((state.S0 | state.S1 | state.S2 | state.S3) == 0)
            throw new ArgumentException("Estado do RNG não pode ser todo zero.", nameof(state));
        (_s0, _s1, _s2, _s3) = (state.S0, state.S1, state.S2, state.S3);
    }

    public static WorldRandom FromState(RandomState state) => new(state);

    public RandomState GetState() => new(_s0, _s1, _s2, _s3);

    public ulong NextULong()
    {
        var result = BitOperations.RotateLeft(_s1 * 5, 7) * 9;
        var t = _s1 << 17;
        _s2 ^= _s0;
        _s3 ^= _s1;
        _s1 ^= _s2;
        _s0 ^= _s3;
        _s2 ^= t;
        _s3 = BitOperations.RotateLeft(_s3, 45);
        return result;
    }

    /// <summary>Inteiro uniforme em [0, maxExclusive).</summary>
    public int NextInt(int maxExclusive)
    {
        if (maxExclusive <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxExclusive), "Deve ser maior que zero.");
        return (int)NextBounded((ulong)maxExclusive);
    }

    /// <summary>Inteiro uniforme em [minInclusive, maxExclusive).</summary>
    public int NextInt(int minInclusive, int maxExclusive)
    {
        if (maxExclusive <= minInclusive)
            throw new ArgumentOutOfRangeException(nameof(maxExclusive), "Deve ser maior que minInclusive.");
        return minInclusive + (int)NextBounded((ulong)((long)maxExclusive - minInclusive));
    }

    /// <summary>Double uniforme em [0, 1).</summary>
    public double NextDouble() => (NextULong() >> 11) * (1.0 / (1UL << 53));

    /// <summary>Retorna true com probabilidade <paramref name="probability"/>.</summary>
    public bool Chance(double probability) => NextDouble() < probability;

    // Método de Lemire: inteiro uniforme sem viés em [0, bound).
    private ulong NextBounded(ulong bound)
    {
        var high = Math.BigMul(NextULong(), bound, out var low);
        if (low < bound)
        {
            var threshold = (0 - bound) % bound;
            while (low < threshold)
                high = Math.BigMul(NextULong(), bound, out low);
        }
        return high;
    }

    private static ulong SplitMix64(ref ulong x)
    {
        var z = x += 0x9E3779B97F4A7C15;
        z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9;
        z = (z ^ (z >> 27)) * 0x94D049BB133111EB;
        return z ^ (z >> 31);
    }
}

public readonly record struct RandomState(ulong S0, ulong S1, ulong S2, ulong S3);
