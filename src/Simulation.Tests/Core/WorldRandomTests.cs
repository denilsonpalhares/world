using World.Simulation.Core;

namespace World.Simulation.Tests.Core;

public class WorldRandomTests
{
    [Fact]
    public void SameSeedProducesSameSequence()
    {
        var a = new WorldRandom(42);
        var b = new WorldRandom(42);

        for (var i = 0; i < 1000; i++)
            Assert.Equal(a.NextULong(), b.NextULong());
    }

    [Fact]
    public void DifferentSeedsProduceDifferentSequences()
    {
        var a = new WorldRandom(1);
        var b = new WorldRandom(2);

        Assert.NotEqual(a.NextULong(), b.NextULong());
    }

    [Fact]
    public void SequenceIsStableAcrossVersions()
    {
        // Valores fixos: se mudarem, saves e replays antigos deixam de ser reproduzíveis.
        var random = new WorldRandom(12345);
        var first = random.NextULong();
        var second = random.NextULong();

        var again = new WorldRandom(12345);
        Assert.Equal(first, again.NextULong());
        Assert.Equal(second, again.NextULong());
        Assert.Equal(StableFirstValue, first);
    }

    // Primeiro valor de xoshiro256** com seed 12345 via SplitMix64 (conferido com implementação de referência).
    private const ulong StableFirstValue = 0xBE6A_3637_4160_D49B;

    [Fact]
    public void RestoringStateContinuesSameSequence()
    {
        var original = new WorldRandom(7);
        for (var i = 0; i < 50; i++)
            original.NextULong();

        var restored = WorldRandom.FromState(original.GetState());

        for (var i = 0; i < 100; i++)
            Assert.Equal(original.NextULong(), restored.NextULong());
    }

    [Fact]
    public void AllZeroStateIsRejected()
    {
        Assert.Throws<ArgumentException>(() => WorldRandom.FromState(new RandomState(0, 0, 0, 0)));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(100)]
    [InlineData(int.MaxValue)]
    public void NextIntStaysInRange(int max)
    {
        var random = new WorldRandom(99);
        for (var i = 0; i < 10_000; i++)
        {
            var value = random.NextInt(max);
            Assert.InRange(value, 0, max - 1);
        }
    }

    [Fact]
    public void NextIntWithMinStaysInRange()
    {
        var random = new WorldRandom(3);
        for (var i = 0; i < 10_000; i++)
            Assert.InRange(random.NextInt(-5, 5), -5, 4);
    }

    [Fact]
    public void NextIntCoversWholeRange()
    {
        var random = new WorldRandom(5);
        var seen = new bool[10];
        for (var i = 0; i < 1000; i++)
            seen[random.NextInt(10)] = true;

        Assert.All(seen, Assert.True);
    }

    [Fact]
    public void NextDoubleStaysInUnitInterval()
    {
        var random = new WorldRandom(11);
        for (var i = 0; i < 10_000; i++)
            Assert.InRange(random.NextDouble(), 0.0, 0.9999999999999999);
    }

    [Fact]
    public void ChanceRespectsExtremes()
    {
        var random = new WorldRandom(13);
        for (var i = 0; i < 1000; i++)
        {
            Assert.False(random.Chance(0.0));
            Assert.True(random.Chance(1.0));
        }
    }

    [Fact]
    public void InvalidRangesThrow()
    {
        var random = new WorldRandom(1);
        Assert.Throws<ArgumentOutOfRangeException>(() => random.NextInt(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => random.NextInt(5, 5));
    }
}
