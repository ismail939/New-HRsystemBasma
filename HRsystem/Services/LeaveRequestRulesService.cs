namespace HRsystem.Services;

public sealed record LeaveRequestValidationInput(
    DateTime StartDate,
    DateTime EndDate,
    int RequestedDays,
    int AvailableDays);

public sealed record LeaveRequestValidationResult(bool IsValid, string? Error);

public class LeaveRequestRulesService
{
    public LeaveRequestValidationResult Validate(LeaveRequestValidationInput input)
    {
        if (input.EndDate.Date < input.StartDate.Date)
            return new(false, "End date cannot be before start date.");
        if (input.RequestedDays <= 0)
            return new(false, "Requested days must be greater than zero.");
        if (input.AvailableDays < input.RequestedDays)
            return new(false, "Leave balance is insufficient.");

        var calendarDays = (input.EndDate.Date - input.StartDate.Date).Days + 1;
        if (input.RequestedDays > calendarDays)
            return new(false, "Requested days exceed the selected date range.");

        return new(true, null);
    }
}
