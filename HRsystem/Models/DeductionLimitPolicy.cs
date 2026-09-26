using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HRsystem.Models
{
    public class DeductionLimitPolicy
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>Null = applies globally</summary>
        [ForeignKey(nameof(PayrollPolicy))]
        public int? PayrollPolicyId { get; set; }

        [JsonIgnore]
        public virtual PayrollPolicy? PayrollPolicy { get; set; }

        [Precision(18, 2)] public decimal? MaxDeductionPerMonth { get; set; }
        [Precision(18, 2)] public decimal? MaxDeductionPerYear { get; set; }
        [Precision(18, 2)] public decimal? MaxMoneyPerPenalty { get; set; }
        [Precision(18, 2)] public decimal? MaxDaysPerPenalty { get; set; }
        [Precision(18, 4)] public decimal? MaxPercentageOfSalary { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
