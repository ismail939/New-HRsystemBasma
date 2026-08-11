using System.ComponentModel.DataAnnotations;
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

        public decimal? MaxDeductionPerMonth { get; set; }
        public decimal? MaxDeductionPerYear { get; set; }
        public decimal? MaxMoneyPerPenalty { get; set; }
        public decimal? MaxDaysPerPenalty { get; set; }
        public decimal? MaxPercentageOfSalary { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}