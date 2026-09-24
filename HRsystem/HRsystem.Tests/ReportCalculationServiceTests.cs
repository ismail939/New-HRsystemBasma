using HRsystem.Services;

namespace HRsystem.Tests;

public class ReportCalculationServiceTests
{
    [Fact]
    public void Calculate_ReturnsNetSalaryAndAttendanceTotals()
    {
        var result = new ReportCalculationService().Calculate(new(
            12_000m, 2_200m, 20, 2, 3, 45, 90));

        Assert.Equal(9_800m, result.NetSalary);
        Assert.Equal(25, result.TotalDays);
        Assert.Equal(1.5m, result.OvertimeHours);
    }

    [Fact]
    public void Calculate_ConvertsOvertimeMinutesToHours()
    {
        var result = new ReportCalculationService().Calculate(new(0m, 0m, 0, 0, 0, 0, 125));

        Assert.Equal(125m / 60m, result.OvertimeHours);
    }

    [Fact]
    public void Calculate_ThrowsWhenSalaryValuesAreNegative()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ReportCalculationService().Calculate(
            new(-1m, 0m, 0, 0, 0, 0, 0)));
    }

    [Fact]
    public void Calculate_AllowsZeroValues()
    {
        var result = new ReportCalculationService().Calculate(new(0m, 0m, 0, 0, 0, 0, 0));

        Assert.Equal(0m, result.NetSalary);
        Assert.Equal(0, result.TotalDays);
        Assert.Equal(0m, result.OvertimeHours);
    }

    [Fact]
    public void Calculate_ReportsNegativeNetSalaryWhenDeductionsExceedGross()
    {
        var result = new ReportCalculationService().Calculate(new(1_000m, 1_500m, 1, 0, 0, 0, 0));

        Assert.Equal(-500m, result.NetSalary);
    }
}
