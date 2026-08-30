using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HRsystem.Models
{
    public class PayrollEarning
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(PayrollDetail))]
        public int PayrollDetailId { get; set; }

        [JsonIgnore]
        public virtual PayrollDetail PayrollDetail { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public decimal Amount { get; set; }

        /// <summary>هل هذا البند يخضع للضريبة؟</summary>
        public bool IsTaxable { get; set; } = true;

        /// <summary>هل هذا البند يخضع للتأمينات؟</summary>
        public bool IsInsurable { get; set; } = true;

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}