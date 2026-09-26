using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
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
        [Precision(18, 2)] public decimal FromAmount { get; set; }

        [Required]
        [Precision(18, 2)] public decimal ToAmount { get; set; }

        [Required]
        [Precision(18, 4)] public decimal Rate { get; set; }
    }
}
