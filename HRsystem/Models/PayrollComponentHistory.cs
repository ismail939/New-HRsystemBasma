using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HRsystem.Models
{
    public class PayrollComponentHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(HREmployee))]
        public int EmployeeId { get; set; }

        [JsonIgnore]
        public virtual HREmployee HREmployee { get; set; }

        [Required]
        [ForeignKey(nameof(PayrollComponent))]
        public int PayrollComponentId { get; set; }

        public virtual PayrollComponent PayrollComponent { get; set; }

        [Precision(18, 2)] public decimal? OldAmount { get; set; }

        [Required]
        [Precision(18, 2)] public decimal NewAmount { get; set; }

        [Required]
        public DateTime EffectiveDate { get; set; }

        [StringLength(500)]
        public string? Reason { get; set; }

        [ForeignKey(nameof(User))]
        public int? ChangedByUserId { get; set; }

        [JsonIgnore]
        public virtual User? ChangedByUser { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
