namespace HRsystem.ViewModels;

public class EmployeeDashboardViewModel
{
    public string EmployeeName { get; set; } = string.Empty;
    public int AnnualBalance { get; set; }
    public int CasualBalance { get; set; }
    public int OffBalance { get; set; }
    public int CompensatoryBalance { get; set; }
    public int ActivePenaltiesCount { get; set; }
    public int TotalPenaltyPoints { get; set; }
    public decimal BasicSalary { get; set; }
    public decimal NetSalary { get; set; }
    public decimal YearToDateEarnings { get; set; }
    public string NextPaymentDate { get; set; } = string.Empty;
    public decimal UpcomingMonthSalary { get; set; }
    public string UpcomingMonthName { get; set; } = string.Empty;
    public List<DayStatus> Days { get; set; } = new();
}

public class DayStatus
{
    public string DayName { get; set; } = string.Empty;
    public string DayNumber { get; set; } = string.Empty;
    public string MonthName { get; set; } = string.Empty;
    public bool IsOffDay { get; set; }
    public string? OffDayType { get; set; }
    public bool IsPast { get; set; }
    public bool IsToday { get; set; }
    public DateTime Date { get; set; }
}