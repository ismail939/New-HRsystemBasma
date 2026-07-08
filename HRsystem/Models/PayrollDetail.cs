using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HRsystem.Models
{
    public class PayrollDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Payroll))]
        public int PayrollId { get; set; }

        [JsonIgnore]
        public virtual Payroll Payroll { get; set; }

        [Required]
        [ForeignKey(nameof(HREmployee))]
        public int EmployeeId { get; set; }

        public virtual HREmployee HREmployee { get; set; }

        // ===== Salary Components =====
        public decimal BasicSalary { get; set; }
        public decimal TotalEarnings { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal NetSalary { get; set; }

        // ===== Attendance =====
        public int PresentDays { get; set; }
        public int AbsentDays { get; set; }
        public float LateMinutes { get; set; }
        public float OvertimeHours { get; set; }
        public int PaidLeaves { get; set; }
        public int UnpaidLeaves { get; set; }
        public int OfficialHolidays { get; set; }

        /// <summary>Daily salary rate used for deductions</summary>
        public decimal DailySalaryRate { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        // Navigation for earnings and deductions breakdown
        public virtual ICollection<PayrollEarning> PayrollEarnings { get; set; } = new List<PayrollEarning>();
        public virtual ICollection<PayrollDeduction> PayrollDeductions { get; set; } = new List<PayrollDeduction>();
    }
}