using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HRsystem.Models
{
    public class CommissionTransaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(HREmployee))]
        public int EmployeeId { get; set; }

        [JsonIgnore]
        public virtual HREmployee HREmployee { get; set; }

        [Required]
        [ForeignKey(nameof(CommissionPlan))]
        public int CommissionPlanId { get; set; }

        public virtual CommissionPlan CommissionPlan { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; }

        [Required]
        [Precision(18, 2)] public decimal Amount { get; set; }

        [Precision(18, 2)] public decimal? BaseAmount { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        /// <summary>Link to payroll item when processed</summary>
        public int? PayrollItemId { get; set; }
    }
}
