using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using HRsystem.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace HRsystem.Models
{
    public class PayrollItem
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

        [JsonIgnore]
        public virtual HREmployee HREmployee { get; set; }

        [Required]
        [ForeignKey(nameof(PayrollComponent))]
        public int PayrollComponentId { get; set; }

        public virtual PayrollComponent PayrollComponent { get; set; }

        [Required]
        [Precision(18, 2)] public decimal Amount { get; set; }

        [Precision(18, 2)] public decimal? Quantity { get; set; }

        [Precision(18, 4)] public decimal? Rate { get; set; }

        /// <summary>True = entered manually, False = auto-generated</summary>
        public bool IsManual { get; set; } = false;

        public bool IsActive { get; set; } = true;

        [Required]
        public PayrollItemSourceType SourceType { get; set; } = PayrollItemSourceType.Recurring;

        public int? SourceId { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
