namespace HRsystem.Models.Enums
{
    public enum PayrollComponentCategory
    {
        Salary = 0,
        Allowance = 1,
        Bonus = 2,
        Incentive = 3,
        Commission = 4,
        Overtime = 5,
        Deduction = 6,
        Other = 7
    }

    public enum CalculationMethod
    {
        FixedAmount = 0,
        Percentage = 1,
        Formula = 2
    }

    public enum DailySalaryCalcMethod
    {
        CalendarDays = 0,
        WorkingDays = 1,
        FixedValue = 2
    }

    public enum OvertimeBase
    {
        BasicSalary = 0,
        GrossSalary = 1
    }

    public enum DeductionMethod
    {
        PerMinute = 0,
        PerOccurrence = 1,
        Graduated = 2
    }

    public enum OvertimeType
    {
        Weekday = 0,
        Weekend = 1,
        Holiday = 2
    }

    public enum CommissionPlanType
    {
        Percentage = 0,
        FixedAmount = 1,
        TargetBased = 2,
        Tiered = 3,
        ProfitBased = 4
    }

    public enum PayrollStatus
    {
        Draft = 0,
        Reviewed = 1,
        Approved = 2,
        Locked = 3
    }

    public enum HourlyRateMethod
    {
        BasicSalary = 0,
        GrossSalary = 1,
        FixedRate = 2
    }

    public enum PayrollItemSourceType
    {
        Recurring = 0,
        Overtime = 1,
        Commission = 2,
        Penalty = 3,
        Manual = 4
    }
}