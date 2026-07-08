using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HRsystem.Models
{
    public class SalaryHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(HREmployee))]
        public int EmployeeId { get; set; }

        [JsonIgnore]
        public virtual HREmployee HREmployee { get; set; }

        /// <summary>The salary component that was changed</summary>
        [ForeignKey(nameof(SalaryComponent))]
        public int? SalaryComponentId { get; set; }

        public virtual SalaryComponent? SalaryComponent { get; set; }

        public decimal? PreviousValue { get; set; }

        [Required]
        public decimal NewValue { get; set; }

        [Required]
        public DateTime EffectiveDate { get; set; }

        [StringLength(500)]
        public string? Reason { get; set; }

        [StringLength(100)]
        public string ChangedBy { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}