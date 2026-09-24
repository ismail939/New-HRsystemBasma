using HRsystem.Models;
using HRsystem.Models.Enums;
using HRsystem.Services;

namespace HRsystem.Tests;

public class PayrollCalculationServiceTests
{
    [Fact]
    public void Calculate_ReturnsGrossTaxInsuranceDeductionsAndNet()
    {
        var policy = new PayrollPolicy { WorkingDaysPerMonth = 20, DailySalaryCalcMethod = DailySalaryCalcMethod.WorkingDays };
        var result = new PayrollCalculationService().Calculate(new(
            10_000m, 12_000m, 500m, 10_000m, 10_000m, policy,
            new[] { new TaxBracket { FromAmount = 0m, ToAmount = null, Rate = 10m } },
            new InsurancePolicy { EmployeeRate = 7m, IsActive = true }));

        Assert.Equal(12_000m, result.GrossSalary);
        Assert.Equal(1_000m, result.TaxAmount);
        Assert.Equal(700m, result.InsuranceAmount);
        Assert.Equal(2_200m, result.TotalDeductions);
        Assert.Equal(9_800m, result.NetSalary);
        Assert.Equal(500m, result.DailySalaryRate);
    }

    [Fact]
    public void CalculateProgressiveTax_UsesMultipleBrackets()
    {
        var result = PayrollCalculationService.CalculateProgressiveTax(30_000m, new[]
        {
            new TaxBracket { FromAmount = 0m, ToAmount = 10_000m, Rate = 10m },
            new TaxBracket { FromAmount = 10_000m, ToAmount = 20_000m, Rate = 20m },
            new TaxBracket { FromAmount = 20_000m, ToAmount = null, Rate = 30m }
        });

        Assert.Equal(6_000m, result);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(10_000, 1_000)]
    [InlineData(15_000, 2_000)]
    [InlineData(20_000, 3_000)]
    public void CalculateProgressiveTax_HandlesBracketBoundaries(decimal taxableAmount, decimal expectedTax)
    {
        var result = PayrollCalculationService.CalculateProgressiveTax(taxableAmount, new[]
        {
            new TaxBracket { FromAmount = 0m, ToAmount = 10_000m, Rate = 10m },
            new TaxBracket { FromAmount = 10_000m, ToAmount = 20_000m, Rate = 20m },
            new TaxBracket { FromAmount = 20_000m, ToAmount = null, Rate = 30m }
        });

        Assert.Equal(expectedTax, result);
    }

    [Fact]
    public void CalculateProgressiveTax_ReturnsZeroForNegativeIncome()
    {
        Assert.Equal(0m, PayrollCalculationService.CalculateProgressiveTax(-1m, Array.Empty<TaxBracket>()));
    }

    [Fact]
    public void CalculateInsurance_ClampsAmountToPolicyLimits()
    {
        var result = PayrollCalculationService.CalculateInsurance(20_000m,
            new InsurancePolicy { MinimumInsurableSalary = 5_000m, MaximumInsurableSalary = 10_000m, EmployeeRate = 10m, IsActive = true });

        Assert.Equal(1_000m, result);
    }

    [Fact]
    public void CalculateInsurance_UsesMinimumWhenSalaryIsBelowMinimum()
    {
        var result = PayrollCalculationService.CalculateInsurance(2_000m,
            new InsurancePolicy { MinimumInsurableSalary = 5_000m, EmployeeRate = 10m, IsActive = true });

        Assert.Equal(500m, result);
    }

    [Fact]
    public void CalculateInsurance_ReturnsZeroForInactivePolicy()
    {
        var result = PayrollCalculationService.CalculateInsurance(10_000m,
            new InsurancePolicy { EmployeeRate = 10m, IsActive = false });

        Assert.Equal(0m, result);
    }

    [Fact]
    public void Calculate_UsesFixedDailySalaryWhenConfigured()
    {
        var policy = new PayrollPolicy
        {
            DailySalaryCalcMethod = DailySalaryCalcMethod.FixedValue,
            DailySalaryFixedValue = 750m,
            WorkingDaysPerMonth = 20
        };

        var result = new PayrollCalculationService().Calculate(new(
            10_000m, 10_000m, 0m, 0m, 0m, policy, Array.Empty<TaxBracket>(), null));

        Assert.Equal(750m, result.DailySalaryRate);
    }

    [Fact(Skip = "Not implemented: FixedDeduction is currently ignored by PayrollCalculationService.")]
    public void Calculate_AppliesFixedTaxDeduction()
    {
        var policy = new PayrollPolicy();
        var result = new PayrollCalculationService().Calculate(new(
            10_000m, 10_000m, 0m, 10_000m, 0m, policy,
            new[] { new TaxBracket { FromAmount = 0m, ToAmount = null, Rate = 10m, FixedDeduction = 100m } }, null));

        Assert.Equal(900m, result.TaxAmount);
    }

    [Fact]
    public void Calculate_IncludesOvertimeUsingTheConfiguredMultiplier()
    {
        var policy = new PayrollPolicy { WorkingDaysPerMonth = 20 };
        var overtimePolicy = new OvertimePolicy
        {
            HourlyRateMethod = HourlyRateMethod.BasicSalary,
            WeekdayMultiplier = 1.5m,
            IsActive = true
        };

        var result = new PayrollCalculationService().Calculate(new(
            10_000m, 10_000m, 0m, 0m, 0m, policy, Array.Empty<TaxBracket>(), null,
            10m, overtimePolicy));

        // (10,000 / 20 / 8) * 1.5 * 10 = 937.50
        Assert.Equal(937.50m, result.OvertimePay);
        Assert.Equal(10_937.50m, result.GrossSalary);
    }

    [Fact]
    public void Calculate_ThrowsWhenPayrollAmountIsNegative()
    {
        var policy = new PayrollPolicy();

        Assert.Throws<ArgumentOutOfRangeException>(() => new PayrollCalculationService().Calculate(new(
            -1m, 0m, 0m, 0m, 0m, policy, Array.Empty<TaxBracket>(), null)));
    }
}
