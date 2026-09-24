using HRsystem.Models.Enums;
using HRsystem.Services;

namespace HRsystem.Tests;

public class PenaltyCalculationServiceTests
{
    [Theory]
    [InlineData(DeductionUnit.Money, 500, 500, 0)]
    [InlineData(DeductionUnit.Day, 2, 2000, 2)]
    [InlineData(DeductionUnit.Hour, 4, 500, 0.5)]
    [InlineData(DeductionUnit.Percentage, 10, 1000, 0)]
    public void Calculate_ReturnsExpectedAmount(DeductionUnit unit, decimal value, decimal expectedAmount, decimal expectedDays)
    {
        var result = new PenaltyCalculationService().Calculate(new(unit, value, 10_000m, 1_000m, 8m));

        Assert.Equal(expectedAmount, result.Amount);
        Assert.Equal(expectedDays == 0 ? null : expectedDays, result.DeductionDays);
    }

    [Fact]
    public void Calculate_WarningOnlyReturnsNoDeduction()
    {
        var result = new PenaltyCalculationService().Calculate(new(DeductionUnit.WarningOnly, 100m, 10_000m, 500m, 8m));

        Assert.Equal(0m, result.Amount);
        Assert.Null(result.DeductionDays);
    }

    [Fact]
    public void Calculate_ThrowsWhenValueIsNegative()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new PenaltyCalculationService().Calculate(
            new(DeductionUnit.Money, -1m, 10_000m, 500m, 8m)));
    }

    [Fact]
    public void Calculate_HourPenaltyReturnsZeroWhenWorkingHoursAreZero()
    {
        var result = new PenaltyCalculationService().Calculate(
            new(DeductionUnit.Hour, 4m, 10_000m, 1_000m, 0m));

        Assert.Equal(0m, result.Amount);
        Assert.Equal(0m, result.DeductionDays);
    }

    [Fact]
    public void Calculate_ZeroPenaltyReturnsZero()
    {
        var result = new PenaltyCalculationService().Calculate(
            new(DeductionUnit.Money, 0m, 10_000m, 1_000m, 8m));

        Assert.Equal(0m, result.Amount);
    }
}
