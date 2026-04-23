namespace HRsystem.ViewModels
{
    public class DailyReportVM
    {
        public DateTime Day { get; set; }
        public List<DailyReportRowVM> Rows { get; set; } = new();
    }

    public class DailyReportRowVM
    {
        public string EmployeeName { get; set; }
         public string DepartmentName { get; set; }
        public string JobName { get; set; }
        public string? ShiftStart { get; set; }
        public string? ShiftEnd { get; set; }

        public string? ArrivalTime { get; set; }
        public string? DepartureTime { get; set; }

        public string? TotalHours { get; set; }
        public string? LateMinutes { get; set; }
        public string? OvertimeMinutes { get; set; }
        public string? EarlyLeaveMinutes { get; set; }

        // ✅ عمود واحد بس للحالة
        public string? Status { get; set; }

        public string? Notes { get; set; }
    }
}