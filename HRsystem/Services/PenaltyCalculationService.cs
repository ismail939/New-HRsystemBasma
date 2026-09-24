using HRsystem.Models.Enums;

namespace HRsystem.Services;

public sealed record PenaltyCalculationInput(
    DeductionUnit Unit,
    decimal Value,
    decimal BasicSalary,
    decimal DailySalaryRate,
    decimal WorkingHoursPerDay);

public sealed record PenaltyCalculationResult(decimal Amount, decimal? DeductionDays);

public class PenaltyCalculationService
{
    public PenaltyCalculationResult Calculate(PenaltyCalculationInput input)
    {
        if (input.Value < 0) throw new ArgumentOutOfRangeException(nameof(input.Value));

        return input.Unit switch
        {
            DeductionUnit.WarningOnly => new(0m, null),
            DeductionUnit.Money => new(input.Value, null),
            DeductionUnit.Day => new(input.Value * input.DailySalaryRate, input.Value),
            DeductionUnit.Hour => input.WorkingHoursPerDay > 0
                ? new(input.Value * input.DailySalaryRate / input.WorkingHoursPerDay,
                    input.Value / input.WorkingHoursPerDay)
                : new(0m, 0m),
            DeductionUnit.Percentage => new(input.Value * input.BasicSalary / 100m, null),
            _ => new(0m, null)
        };
    }
}
