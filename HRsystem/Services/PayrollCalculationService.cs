using HRsystem.Models;
using HRsystem.Models.Enums;

namespace HRsystem.Services;

public sealed record PayrollCalculationInput(
    decimal BasicSalary,
    decimal Earnings,
    decimal FixedDeductions,
    decimal TaxableAmount,
    decimal InsurableAmount,
    PayrollPolicy Policy,
    IReadOnlyCollection<TaxBracket> TaxBrackets,
    InsurancePolicy? InsurancePolicy,
    decimal OvertimeHours = 0m,
    OvertimePolicy? OvertimePolicy = null,
    decimal GrossSalaryForOvertime = 0m);

public sealed record PayrollCalculationResult(
    decimal GrossSalary,
    decimal TaxAmount,
    decimal InsuranceAmount,
    decimal TotalDeductions,
    decimal NetSalary,
    decimal DailySalaryRate,
    decimal OvertimePay);

public class PayrollCalculationService
{
    public decimal CalculateDailySalaryRate(decimal basicSalary, PayrollPolicy policy, decimal workingDays)
    {
        if (basicSalary < 0) throw new ArgumentOutOfRangeException(nameof(basicSalary));

        return policy.DailySalaryCalcMethod == Models.Enums.DailySalaryCalcMethod.FixedValue
            ? policy.DailySalaryFixedValue ?? 0m
            : workingDays > 0 ? basicSalary / workingDays : 0m;
    }

    public PayrollCalculationResult Calculate(PayrollCalculationInput input)
    {
        if (input.BasicSalary < 0 || input.Earnings < 0 || input.FixedDeductions < 0)
            throw new ArgumentOutOfRangeException(nameof(input), "Payroll amounts cannot be negative.");

        var workingDays = input.Policy.DailySalaryCalcMethod == Models.Enums.DailySalaryCalcMethod.CalendarDays
            ? input.Policy.CalendarDaysPerMonth
            : input.Policy.WorkingDaysPerMonth;
        var dailyRate = CalculateDailySalaryRate(input.BasicSalary, input.Policy, workingDays);
        var overtimePay = CalculateOvertimePay(input.BasicSalary, input.Earnings, input.OvertimeHours, input.OvertimePolicy, dailyRate);
        var grossSalary = input.Earnings + overtimePay;

        var tax = CalculateProgressiveTax(input.TaxableAmount + overtimePay, input.TaxBrackets);
        var insurance = CalculateInsurance(input.InsurableAmount + overtimePay, input.InsurancePolicy);
        var deductions = input.FixedDeductions + tax + insurance;

        return new PayrollCalculationResult(
            grossSalary,
            tax,
            insurance,
            deductions,
            grossSalary - deductions,
            dailyRate,
            overtimePay);
    }

    public static decimal CalculateOvertimePay(
        decimal basicSalary,
        decimal grossSalary,
        decimal overtimeHours,
        OvertimePolicy? policy,
        decimal dailySalaryRate,
        OvertimeType overtimeType = OvertimeType.Weekday)
    {
        if (overtimeHours <= 0 || policy is null || !policy.IsActive) return 0m;
        if (policy.MaxOvertimeHoursPerMonth.HasValue)
            overtimeHours = Math.Min(overtimeHours, policy.MaxOvertimeHoursPerMonth.Value);
        if (overtimeHours <= 0) return 0m;

        var hourlyRate = policy.HourlyRateMethod switch
        {
            HourlyRateMethod.FixedRate => policy.FixedHourlyRate ?? 0m,
            HourlyRateMethod.GrossSalary => grossSalary / 22m / 8m,
            _ => dailySalaryRate / 8m
        };

        var multiplier = overtimeType switch
        {
            OvertimeType.Weekend => policy.WeekendMultiplier,
            OvertimeType.Holiday => policy.HolidayMultiplier,
            _ => policy.WeekdayMultiplier
        };

        return Math.Round(overtimeHours * hourlyRate * multiplier, 2, MidpointRounding.AwayFromZero);
    }

    public static decimal CalculateProgressiveTax(decimal taxableAmount, IEnumerable<TaxBracket> brackets)
    {
        if (taxableAmount <= 0) return 0m;

        decimal tax = 0m;
        foreach (var bracket in brackets.OrderBy(b => b.FromAmount))
        {
            var upper = bracket.ToAmount ?? taxableAmount;
            var amount = Math.Max(0m, Math.Min(taxableAmount, upper) - bracket.FromAmount);
            if (amount > 0) tax += amount * bracket.Rate / 100m;
            if (taxableAmount <= upper) break;
        }

        return Math.Round(tax, 2, MidpointRounding.AwayFromZero);
    }

    public static decimal CalculateInsurance(decimal insurableAmount, InsurancePolicy? policy)
    {
        if (policy is null || !policy.IsActive || insurableAmount <= 0) return 0m;

        var minimum = policy.MinimumInsurableSalary ?? 0m;
        var maximum = policy.MaximumInsurableSalary ?? decimal.MaxValue;
        var insuredBase = Math.Clamp(insurableAmount, minimum, maximum);
        return Math.Round(insuredBase * policy.EmployeeRate / 100m, 2, MidpointRounding.AwayFromZero);
    }
}
