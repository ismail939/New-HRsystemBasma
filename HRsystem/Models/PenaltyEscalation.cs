using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using HRsystem.Models.Enums;

namespace HRsystem.Models
{
    public class PenaltyEscalation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(PenaltyRule))]
        public int PenaltyRuleId { get; set; }

        [JsonIgnore]
        public virtual PenaltyRule PenaltyRule { get; set; }

        [Required]
        public int OccurrenceCount { get; set; }

        [Required]
        public DeductionUnit DeductionUnit { get; set; } = DeductionUnit.WarningOnly;

        public decimal DeductionValue { get; set; }

        public decimal? DeductionAmount { get; set; }

        public decimal? DeductionPercentage { get; set; }

        public bool IsWarning { get; set; } = false;

        [StringLength(200)]
        public string? ActionRequired { get; set; }
    }
}