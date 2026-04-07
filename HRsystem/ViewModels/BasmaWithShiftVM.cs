namespace HRsystem.ViewModels
{
    public class BasmaWithShiftVM
    {
        public DateTime DayDate { get; set; }
        public DateTime? Arrival { get; set; }
        public DateTime? Departure { get; set; }
        public DateTime? ShiftStart { get; set; }
        public DateTime? ShiftEnd { get; set; }
        public string? Notes { get; set; }
        public float? TotalHours { get; set; }
        public int? LateMinutes { get; set; }
        public int? EarlyLeaveMinutes { get; set; }
        public int? OvertimeMinutes { get; set; }
        public int Status { get; set; }
        public int ShiftId { get; set; }
    }
}