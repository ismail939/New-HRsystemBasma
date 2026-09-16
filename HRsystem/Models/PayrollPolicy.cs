using System.ComponentModel.DataAnnotations;
using HRsystem.Models.Enums;

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
        public decimal WorkingDaysPerMonth { get; set; } = 22;

        [Required]
        public decimal CalendarDaysPerMonth { get; set; } = 30;

        [Required]
        public decimal WorkingHoursPerDay { get; set; } = 8;

        [Required]
        public DailySalaryCalcMethod DailySalaryCalcMethod { get; set; } = DailySalaryCalcMethod.WorkingDays;

        public decimal? DailySalaryFixedValue { get; set; }

        [Required]
        public OvertimeBase OvertimeBase { get; set; } = OvertimeBase.BasicSalary;

        [Required]
        public decimal OvertimeWeekdayMultiplier { get; set; } = 1.5m;

        [Required]
        public decimal OvertimeWeekendMultiplier { get; set; } = 2.0m;

        [Required]
        public decimal OvertimeHolidayMultiplier { get; set; } = 3.0m;

        [Required]
        public DeductionMethod LateDeductionMethod { get; set; } = DeductionMethod.PerMinute;

        [Required]
        public DeductionMethod AbsenceDeductionMethod { get; set; } = DeductionMethod.PerOccurrence;

        [Required]
        public DailySalaryCalcMethod LeaveEncashmentMethod { get; set; } = DailySalaryCalcMethod.WorkingDays;

        // Maximum deduction limits
        public decimal? MaxDeductionPerMonth { get; set; }
        public decimal? MaxDeductionPerYear { get; set; }
        public decimal? MaxDeductionAmount { get; set; }
        public decimal? MaxDeductionDays { get; set; }
        public decimal? MaxDeductionPercentage { get; set; }

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
