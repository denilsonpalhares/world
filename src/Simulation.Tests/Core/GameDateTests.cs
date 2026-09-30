using World.Simulation.Core;

namespace World.Simulation.Tests.Core;

public class GameDateTests
{
    [Fact]
    public void TickZeroIsFirstDayOfStartYear()
    {
        var date = new GameDate(0, 1800);

        Assert.Equal((1800, 1, 1), (date.Year, date.Month, date.Day));
        Assert.True(date.IsFirstDayOfMonth);
        Assert.True(date.IsFirstDayOfYear);
        Assert.Equal("1800-01-01", date.ToString());
    }

    [Theory]
    [InlineData(29, 1800, 1, 30)]
    [InlineData(30, 1800, 2, 1)]
    [InlineData(359, 1800, 12, 30)]
    [InlineData(360, 1801, 1, 1)]
    [InlineData(360 * 10 + 45, 1810, 2, 16)]
    public void TickMapsToCalendarDate(long tick, int year, int month, int day)
    {
        var date = new GameDate(tick, 1800);

        Assert.Equal((year, month, day), (date.Year, date.Month, date.Day));
    }

    [Theory]
    [InlineData(1, Season.Winter)]
    [InlineData(2, Season.Winter)]
    [InlineData(3, Season.Spring)]
    [InlineData(5, Season.Spring)]
    [InlineData(6, Season.Summer)]
    [InlineData(8, Season.Summer)]
    [InlineData(9, Season.Autumn)]
    [InlineData(11, Season.Autumn)]
    [InlineData(12, Season.Winter)]
    public void MonthMapsToSeason(int month, Season expected)
    {
        var date = new GameDate((month - 1) * GameDate.DaysPerMonth, 1800);

        Assert.Equal(expected, date.Season);
    }

    [Fact]
    public void NegativeTickThrows()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new GameDate(-1, 1800));
    }
}
