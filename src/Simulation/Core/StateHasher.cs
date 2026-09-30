namespace World.Simulation.Core;

/// <summary>
/// Hash estável (FNV-1a 64 bits) do estado do mundo. Usado para verificar determinismo:
/// mesma seed + mesmos comandos devem produzir o mesmo checksum.
/// </summary>
public sealed class StateHasher
{
    private const ulong OffsetBasis = 14695981039346656037;
    private const ulong Prime = 1099511628211;

    private ulong _hash = OffsetBasis;

    public ulong Value => _hash;

    public StateHasher Add(ulong value)
    {
        for (var i = 0; i < 8; i++)
        {
            _hash ^= (byte)(value >> (i * 8));
            _hash *= Prime;
        }
        return this;
    }

    public StateHasher Add(long value) => Add((ulong)value);

    public StateHasher Add(int value) => Add((ulong)(long)value);

    public StateHasher Add(bool value) => Add(value ? 1UL : 0UL);

    public StateHasher Add(double value) => Add((ulong)BitConverter.DoubleToInt64Bits(value));

    public StateHasher Add(string value)
    {
        Add(value.Length);
        foreach (var c in value)
            Add((ulong)c);
        return this;
    }
}
