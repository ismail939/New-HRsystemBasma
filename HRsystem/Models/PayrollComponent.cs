using System.ComponentModel.DataAnnotations;
using HRsystem.Models.Enums;

namespace HRsystem.Models
{
    public class PayrollComponent
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string NameAr { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        public PayrollComponentCategory Category { get; set; } = PayrollComponentCategory.Allowance;

        /// <summary>True = auto-copied to each payroll run</summary>
        public bool IsRecurring { get; set; } = true;

        [Required]
        public CalculationMethod CalculationMethod { get; set; } = CalculationMethod.FixedAmount;

        public decimal? DefaultAmount { get; set; }

        public decimal? DefaultPercentage { get; set; }

        [StringLength(500)]
        public string? FormulaExpression { get; set; }

        /// <summary>Subject to income tax</summary>
        public bool IsTaxable { get; set; } = true;

        /// <summary>Subject to social insurance</summary>
        public bool IsInsurable { get; set; } = true;

        public bool IsActive { get; set; } = true;

        public int SortOrder { get; set; } = 0;

        [StringLength(500)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public virtual ICollection<EmployeePayrollComponent> EmployeeAssignments { get; set; } = new List<EmployeePayrollComponent>();
        public virtual ICollection<PayrollItem> PayrollItems { get; set; } = new List<PayrollItem>();
    }
}