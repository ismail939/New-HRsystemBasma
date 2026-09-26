using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using HRsystem.Models.Enums;

namespace HRsystem.Models
{
    public class OvertimeEntry
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(HREmployee))]
        public int EmployeeId { get; set; }

        [JsonIgnore]
        public virtual HREmployee HREmployee { get; set; }

        [Required]
        [ForeignKey(nameof(OvertimePolicy))]
        public int OvertimePolicyId { get; set; }

        public virtual OvertimePolicy OvertimePolicy { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [Precision(18, 2)] public decimal OvertimeMinutes { get; set; }

        [Precision(18, 4)] public decimal Rate { get; set; }

        [Precision(18, 4)] public decimal Multiplier { get; set; }

        [Precision(18, 2)] public decimal Amount { get; set; }

        [Required]
        public OvertimeType OvertimeType { get; set; } = OvertimeType.Weekday;

        public bool IsApproved { get; set; } = false;

        [StringLength(100)]
        public string? ApprovedBy { get; set; }

        public DateTime? ApprovedDate { get; set; }

        /// <summary>Link to payroll item when processed</summary>
        public int? PayrollItemId { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
