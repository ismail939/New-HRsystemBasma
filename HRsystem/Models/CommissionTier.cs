using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HRsystem.Models
{
    public class CommissionTier
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(CommissionPlan))]
        public int CommissionPlanId { get; set; }

        [JsonIgnore]
        public virtual CommissionPlan CommissionPlan { get; set; }

        [Required]
        public decimal FromAmount { get; set; }

        [Required]
        public decimal ToAmount { get; set; }

        [Required]
        public decimal Rate { get; set; }
    }
}