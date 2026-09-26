using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
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

        [Precision(18, 2)] public decimal FromValue { get; set; }

        [Precision(18, 2)] public decimal ToValue { get; set; }

        [Required]
        public PenaltyValueType ValueType { get; set; } = PenaltyValueType.Minutes;

        [Required]
        public DeductionUnit DeductionUnit { get; set; } = DeductionUnit.WarningOnly;

        [Precision(18, 2)] public decimal DeductionValue { get; set; }

        [Precision(18, 2)] public decimal? DeductionAmount { get; set; }

        [Precision(18, 4)] public decimal? DeductionPercentage { get; set; }

        public bool IsWarning { get; set; } = false;
    }
}
