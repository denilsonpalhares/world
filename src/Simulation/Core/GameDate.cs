namespace World.Simulation.Core;

public enum Season
{
    Winter,
    Spring,
    Summer,
    Autumn,
}

/// <summary>
/// Data do jogo derivada do tick (1 tick = 1 dia). Calendário simplificado: 12 meses de 30 dias (360 dias/ano),
/// o que mantém meses e estações com duração uniforme.
/// </summary>
public readonly record struct GameDate
{
    public const int DaysPerMonth = 30;
    public const int MonthsPerYear = 12;
    public const int DaysPerYear = DaysPerMonth * MonthsPerYear;

    public GameDate(long tick, int startYear)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(tick);
        Tick = tick;
        StartYear = startYear;
    }

    public long Tick { get; }
    public int StartYear { get; }

    public int Year => StartYear + (int)(Tick / DaysPerYear);
    public int Month => (int)(Tick % DaysPerYear / DaysPerMonth) + 1;
    public int Day => (int)(Tick % DaysPerMonth) + 1;

    public bool IsFirstDayOfMonth => Tick % DaysPerMonth == 0;
    public bool IsFirstDayOfYear => Tick % DaysPerYear == 0;

    public Season Season => Month switch
    {
        12 or 1 or 2 => Season.Winter,
        >= 3 and <= 5 => Season.Spring,
        >= 6 and <= 8 => Season.Summer,
        _ => Season.Autumn,
    };

    public override string ToString() => $"{Year:D4}-{Month:D2}-{Day:D2}";
}
