using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using HRsystem.Models.Enums;

namespace HRsystem.Models
{
    public class OvertimePolicy
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public HourlyRateMethod HourlyRateMethod { get; set; } = HourlyRateMethod.BasicSalary;

        [Precision(18, 4)] public decimal? FixedHourlyRate { get; set; }

        [Required]
        [Precision(18, 4)] public decimal WeekdayMultiplier { get; set; } = 1.5m;

        [Required]
        [Precision(18, 4)] public decimal WeekendMultiplier { get; set; } = 2.0m;

        [Required]
        [Precision(18, 4)] public decimal HolidayMultiplier { get; set; } = 3.0m;

        public int MinOvertimeMinutes { get; set; } = 30;

        [Precision(18, 2)] public decimal? MaxOvertimeHoursPerDay { get; set; }

        [Precision(18, 2)] public decimal? MaxOvertimeHoursPerMonth { get; set; }

        public bool RequiresApproval { get; set; } = true;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public virtual ICollection<EmployeeOvertimePolicy> EmployeeAssignments { get; set; } = new List<EmployeeOvertimePolicy>();
        public virtual ICollection<OvertimeEntry> OvertimeEntries { get; set; } = new List<OvertimeEntry>();
    }
}
