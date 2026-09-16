namespace HRsystem.ViewModels;

public class EmployeeDashboardViewModel
{
    public string EmployeeName { get; set; } = string.Empty;
    
    // All 9 leave type balances
    public int AnnualBalance { get; set; }
    public int CasualBalance { get; set; }
    public int SickBalance { get; set; }
    public int HajjBalance { get; set; }
    public int MaternityBalance { get; set; }
    public int UnpaidBalance { get; set; }
    public int CompensatoryBalance { get; set; }
    public int OfficialHolidayBalance { get; set; }
    public int ExamBalance { get; set; }
    
    // Keep old name for backward compatibility
    public int OffBalance { get; set; } // for unpaid leave
    
    public int ActivePenaltiesCount { get; set; }
    public int TotalPenaltyPoints { get; set; }
    public decimal BasicSalary { get; set; }
    public decimal NetSalary { get; set; }
    public decimal YearToDateEarnings { get; set; }
    public string NextPaymentDate { get; set; } = string.Empty;
    public decimal UpcomingMonthSalary { get; set; }
    public string UpcomingMonthName { get; set; } = string.Empty;
    public List<DayStatus> Days { get; set; } = new();
    public List<EmployeeSalaryHistoryItem> SalaryHistory { get; set; } = new();
    public List<EmployeePayslipItem> Payslips { get; set; } = new();
}

public class EmployeeSalaryHistoryItem
{
    public DateTime EffectiveDate { get; set; }
    public string ComponentName { get; set; } = string.Empty;
    public decimal? OldAmount { get; set; }
    public decimal NewAmount { get; set; }
    public string? Reason { get; set; }
}

public class EmployeePayslipItem
{
    public int PayrollId { get; set; }
    public string Period { get; set; } = string.Empty;
    public decimal BasicSalary { get; set; }
    public decimal GrossSalary { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal InsuranceAmount { get; set; }
    public decimal NetSalary { get; set; }
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
