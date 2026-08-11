using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using HRsystem.Models.Enums;

namespace HRsystem.Models
{
    public class EmployeePenalty
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(HREmployee))]
        public int EmployeeId { get; set; }

        [JsonIgnore]
        public virtual HREmployee HREmployee { get; set; }

        [Required]
        [ForeignKey(nameof(PenaltyRule))]
        public int PenaltyRuleId { get; set; }

        public virtual PenaltyRule PenaltyRule { get; set; }

        [ForeignKey(nameof(PenaltyLevel))]
        public int? PenaltyLevelId { get; set; }

        public virtual PenaltyLevel? PenaltyLevel { get; set; }

        [ForeignKey(nameof(PenaltyEscalation))]
        public int? PenaltyEscalationId { get; set; }

        public virtual PenaltyEscalation? PenaltyEscalation { get; set; }

        [Required]
        public DateTime IncidentDate { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required]
        public PenaltyStatus Status { get; set; } = PenaltyStatus.Draft;

        // Snapshot values at time of creation
        public DeductionUnit DeductionUnit { get; set; } = DeductionUnit.WarningOnly;

        public decimal DeductionValue { get; set; }

        public decimal? DeductionDays { get; set; }

        public decimal? DeductionAmount { get; set; }

        [StringLength(500)]
        public string? ManagerNotes { get; set; }

        [ForeignKey(nameof(ApprovedByUser))]
        public int? ApprovedByUserId { get; set; }

        [JsonIgnore]
        public virtual User? ApprovedByUser { get; set; }

        public DateTime? ApprovedDate { get; set; }

        [StringLength(500)]
        public string? RejectedReason { get; set; }

        /// <summary>Link to payroll item when processed</summary>
        public int? PayrollItemId { get; set; }
    }
}