using System.ComponentModel.DataAnnotations;
using HRsystem.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace HRsystem.Models
{
    public class PayrollPolicy
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Precision(18, 2)] public decimal WorkingDaysPerMonth { get; set; } = 22;

        [Required]
        [Precision(18, 2)] public decimal CalendarDaysPerMonth { get; set; } = 30;

        [Required]
        [Precision(18, 2)] public decimal WorkingHoursPerDay { get; set; } = 8;

        [Required]
        public DailySalaryCalcMethod DailySalaryCalcMethod { get; set; } = DailySalaryCalcMethod.WorkingDays;

        [Precision(18, 2)] public decimal? DailySalaryFixedValue { get; set; }

        [Required]
        public OvertimeBase OvertimeBase { get; set; } = OvertimeBase.BasicSalary;

        [Required]
        [Precision(18, 4)] public decimal OvertimeWeekdayMultiplier { get; set; } = 1.5m;

        [Required]
        [Precision(18, 4)] public decimal OvertimeWeekendMultiplier { get; set; } = 2.0m;

        [Required]
        [Precision(18, 4)] public decimal OvertimeHolidayMultiplier { get; set; } = 3.0m;

        [Required]
        public DeductionMethod LateDeductionMethod { get; set; } = DeductionMethod.PerMinute;

        [Required]
        public DeductionMethod AbsenceDeductionMethod { get; set; } = DeductionMethod.PerOccurrence;

        [Required]
        public DailySalaryCalcMethod LeaveEncashmentMethod { get; set; } = DailySalaryCalcMethod.WorkingDays;

        // Maximum deduction limits
        [Precision(18, 2)] public decimal? MaxDeductionPerMonth { get; set; }
        [Precision(18, 2)] public decimal? MaxDeductionPerYear { get; set; }
        [Precision(18, 2)] public decimal? MaxDeductionAmount { get; set; }
        [Precision(18, 2)] public decimal? MaxDeductionDays { get; set; }
        [Precision(18, 4)] public decimal? MaxDeductionPercentage { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public virtual ICollection<EmployeePayrollPolicy> EmployeeAssignments { get; set; } = new List<EmployeePayrollPolicy>();
        public virtual ICollection<DepartmentPayrollPolicy> DepartmentAssignments { get; set; } = new List<DepartmentPayrollPolicy>();
        public virtual ICollection<Payroll> Payrolls { get; set; } = new List<Payroll>();
        public virtual ICollection<TaxBracket> TaxBrackets { get; set; } = new List<TaxBracket>();
        public virtual ICollection<InsurancePolicy> InsurancePolicies { get; set; } = new List<InsurancePolicy>();
    }
}
