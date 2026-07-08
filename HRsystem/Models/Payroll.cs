using System.ComponentModel.DataAnnotations;

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

        /// <summary>Draft / Reviewed / Approved / Locked</summary>
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Draft";

        public DateTime GeneratedDate { get; set; } = DateTime.Now;

        [StringLength(100)]
        public string? GeneratedBy { get; set; }

        public DateTime? ReviewedDate { get; set; }

        [StringLength(100)]
        public string? ReviewedBy { get; set; }

        public DateTime? ApprovedDate { get; set; }

        [StringLength(100)]
        public string? ApprovedBy { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        // Navigation
        public virtual ICollection<PayrollDetail> PayrollDetails { get; set; } = new List<PayrollDetail>();
    }
}