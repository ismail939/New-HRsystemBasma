namespace HRsystem.Services;

public sealed record ReportCalculationInput(
    decimal GrossSalary,
    decimal TotalDeductions,
    int PresentDays,
    int AbsentDays,
    int LeaveDays,
    int LateMinutes,
    int OvertimeMinutes);

public sealed record ReportCalculationResult(
    decimal NetSalary,
    int TotalDays,
    decimal OvertimeHours);

public class ReportCalculationService
{
    public ReportCalculationResult Calculate(ReportCalculationInput input)
    {
        if (input.GrossSalary < 0 || input.TotalDeductions < 0)
            throw new ArgumentOutOfRangeException(nameof(input), "Report salary values cannot be negative.");

        return new ReportCalculationResult(
            input.GrossSalary - input.TotalDeductions,
            input.PresentDays + input.AbsentDays + input.LeaveDays,
            input.OvertimeMinutes / 60m);
    }
}
