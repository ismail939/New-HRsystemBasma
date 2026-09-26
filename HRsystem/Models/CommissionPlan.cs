using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using HRsystem.Models.Enums;

namespace HRsystem.Models
{
    public class CommissionPlan
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public CommissionPlanType Type { get; set; } = CommissionPlanType.Percentage;

        [Precision(18, 4)] public decimal Value { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public virtual ICollection<CommissionTier> Tiers { get; set; } = new List<CommissionTier>();
        public virtual ICollection<EmployeeCommissionPlan> EmployeeAssignments { get; set; } = new List<EmployeeCommissionPlan>();
        public virtual ICollection<CommissionTransaction> Transactions { get; set; } = new List<CommissionTransaction>();
    }
}
