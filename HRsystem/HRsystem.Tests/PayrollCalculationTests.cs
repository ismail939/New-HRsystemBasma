using System.Reflection;
using HRsystem.Controllers;
using HRsystem.Models;

namespace HRsystem.Tests;

public class PayrollCalculationTests
{
    private static decimal CalculateProgressiveTax(
        decimal taxableAmount,
        List<TaxBracket> brackets)
    {
        MethodInfo method = typeof(PayrollController).GetMethod(
            "CalculateProgressiveTax",
            BindingFlags.NonPublic | BindingFlags.Static)!;

        return (decimal)method.Invoke(null, new object[] { taxableAmount, brackets })!;
    }

    [Fact]
    public void CalculateProgressiveTax_ReturnsZeroForZeroIncome()
    {
        decimal result = CalculateProgressiveTax(0m, new List<TaxBracket>
        {
            new() { FromAmount = 0m, ToAmount = 10_000m, Rate = 10m }
        });

        Assert.Equal(0m, result);
    }

    [Fact]
    public void CalculateProgressiveTax_ReturnsZeroWhenThereAreNoBrackets()
    {
        decimal result = CalculateProgressiveTax(10_000m, new List<TaxBracket>());

        Assert.Equal(0m, result);
    }

    [Fact]
    public void CalculateProgressiveTax_CalculatesOneBracket()
    {
        decimal result = CalculateProgressiveTax(10_000m, new List<TaxBracket>
        {
            new() { FromAmount = 0m, ToAmount = 20_000m, Rate = 10m }
        });

        Assert.Equal(1_000m, result);
    }

    [Fact]
    public void CalculateProgressiveTax_CalculatesMultipleBracketsProgressively()
    {
        decimal result = CalculateProgressiveTax(30_000m, new List<TaxBracket>
        {
            new() { FromAmount = 0m, ToAmount = 10_000m, Rate = 10m },
            new() { FromAmount = 10_000m, ToAmount = 20_000m, Rate = 20m },
            new() { FromAmount = 20_000m, ToAmount = null, Rate = 30m }
        });

        // 1,000 + 2,000 + 3,000
        Assert.Equal(6_000m, result);
    }

    [Fact]
    public void CalculateProgressiveTax_RoundsToTwoDecimalPlaces()
    {
        decimal result = CalculateProgressiveTax(123.45m, new List<TaxBracket>
        {
            new() { FromAmount = 0m, ToAmount = null, Rate = 12.345m }
        });

        Assert.Equal(15.24m, result);
    }
}
