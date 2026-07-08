using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HRsystem.Models
{
    public class PayrollDeduction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(PayrollDetail))]
        public int PayrollDetailId { get; set; }

        [JsonIgnore]
        public virtual PayrollDetail PayrollDetail { get; set; }

        [ForeignKey(nameof(SalaryComponent))]
        public int? SalaryComponentId { get; set; }

        public virtual SalaryComponent? SalaryComponent { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public decimal Amount { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}