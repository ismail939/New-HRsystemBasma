using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

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
        [Precision(18, 2)] public decimal BasicSalary { get; set; }
        [Precision(18, 2)] public decimal TotalEarnings { get; set; }
        [Precision(18, 2)] public decimal TotalDeductions { get; set; }
        [Precision(18, 2)] public decimal GrossSalary { get; set; }
        [Precision(18, 2)] public decimal NetSalary { get; set; }

        // ===== Tax & Insurance =====
        /// <summary>إجمالي المكونات الخاضعة للضريبة</summary>
        [Precision(18, 2)] public decimal TaxableAmount { get; set; }

        /// <summary>إجمالي المكونات الخاضعة للتأمينات</summary>
        [Precision(18, 2)] public decimal InsurableAmount { get; set; }

        /// <summary>قيمة ضريبة الدخل المستقطعة</summary>
        [Precision(18, 2)] public decimal TaxAmount { get; set; }

        /// <summary>قيمة التأمينات الاجتماعية المستقطعة</summary>
        [Precision(18, 2)] public decimal InsuranceAmount { get; set; }

        // ===== Attendance =====
        public int PresentDays { get; set; }
        public int AbsentDays { get; set; }
        public double LateMinutes { get; set; }
        public double OvertimeHours { get; set; }
        public int PaidLeaves { get; set; }
        public int UnpaidLeaves { get; set; }
        public int OfficialHolidays { get; set; }

        /// <summary>Daily salary rate used for deductions</summary>
        [Precision(18, 2)] public decimal DailySalaryRate { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        // Navigation for earnings and deductions breakdown
        public virtual ICollection<PayrollEarning> PayrollEarnings { get; set; } = new List<PayrollEarning>();
        public virtual ICollection<PayrollDeduction> PayrollDeductions { get; set; } = new List<PayrollDeduction>();
    }
}
