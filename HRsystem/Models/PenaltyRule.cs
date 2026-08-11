using System.ComponentModel.DataAnnotations;
using HRsystem.Models.Enums;

namespace HRsystem.Models
{
    public class PenaltyRule
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string NameAr { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        public PenaltyCategory Category { get; set; } = PenaltyCategory.Attendance;

        public bool IsActive { get; set; } = true;

        [StringLength(500)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public virtual ICollection<PenaltyLevel> Levels { get; set; } = new List<PenaltyLevel>();
        public virtual ICollection<PenaltyEscalation> Escalations { get; set; } = new List<PenaltyEscalation>();
        public virtual ICollection<EmployeePenalty> EmployeePenalties { get; set; } = new List<EmployeePenalty>();
    }
}