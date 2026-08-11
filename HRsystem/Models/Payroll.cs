using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using HRsystem.Models.Enums;

namespace HRsystem.Models
{
    public class Payroll
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Month { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public PayrollStatus Status { get; set; } = PayrollStatus.Draft;

        [ForeignKey(nameof(PayrollPolicy))]
        public int? PayrollPolicyId { get; set; }

        [JsonIgnore]
        public virtual PayrollPolicy? PayrollPolicy { get; set; }

        public DateTime GeneratedDate { get; set; } = DateTime.Now;

        [StringLength(100)]
        public string? GeneratedBy { get; set; }

        public DateTime? ReviewedDate { get; set; }

        [StringLength(100)]
        public string? ReviewedBy { get; set; }

        public DateTime? ApprovedDate { get; set; }

        [StringLength(100)]
        public string? ApprovedBy { get; set; }

        public DateTime? LockedDate { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        // Navigation
        public virtual ICollection<PayrollItem> PayrollItems { get; set; } = new List<PayrollItem>();
    }
}