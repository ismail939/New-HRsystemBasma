using HRsystem.Models.Enums;

namespace HRsystem.ViewModels
{
    public class PayrollListViewModel
    {
        public int Id { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public PayrollStatus Status { get; set; }
        public DateTime GeneratedDate { get; set; }
        public string GeneratedBy { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public int EmployeeCount { get; set; }
    }
}