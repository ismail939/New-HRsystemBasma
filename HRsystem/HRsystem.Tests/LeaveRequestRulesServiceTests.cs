using HRsystem.Services;

namespace HRsystem.Tests;

public class LeaveRequestRulesServiceTests
{
    private readonly LeaveRequestRulesService _service = new();

    [Fact]
    public void Validate_AcceptsValidRequest()
    {
        var result = _service.Validate(new(new(2026, 9, 10), new(2026, 9, 12), 3, 10));

        Assert.True(result.IsValid);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Validate_RejectsEndDateBeforeStartDate()
    {
        var result = _service.Validate(new(new(2026, 9, 12), new(2026, 9, 10), 1, 10));

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_RejectsInsufficientBalance()
    {
        var result = _service.Validate(new(new(2026, 9, 10), new(2026, 9, 12), 4, 3));

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_RejectsRequestedDaysGreaterThanDateRange()
    {
        var result = _service.Validate(new(new(2026, 9, 10), new(2026, 9, 12), 4, 10));

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_RejectsZeroRequestedDays()
    {
        var result = _service.Validate(new(new(2026, 9, 10), new(2026, 9, 10), 0, 10));

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_AcceptsOneDayLeave()
    {
        var result = _service.Validate(new(new(2026, 9, 10), new(2026, 9, 10), 1, 1));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_RejectsNegativeBalance()
    {
        var result = _service.Validate(new(new(2026, 9, 10), new(2026, 9, 10), 1, -1));

        Assert.False(result.IsValid);
    }
}
