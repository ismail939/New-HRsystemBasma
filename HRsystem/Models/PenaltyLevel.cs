using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using HRsystem.Models.Enums;

namespace HRsystem.Models
{
    public class PenaltyLevel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(PenaltyRule))]
        public int PenaltyRuleId { get; set; }

        [JsonIgnore]
        public virtual PenaltyRule PenaltyRule { get; set; }

        [Required]
        public int SequenceOrder { get; set; }

        public decimal FromValue { get; set; }

        public decimal ToValue { get; set; }

        [Required]
        public PenaltyValueType ValueType { get; set; } = PenaltyValueType.Minutes;

        [Required]
        public DeductionUnit DeductionUnit { get; set; } = DeductionUnit.WarningOnly;

        public decimal DeductionValue { get; set; }

        public decimal? DeductionAmount { get; set; }

        public decimal? DeductionPercentage { get; set; }

        public bool IsWarning { get; set; } = false;
    }
}