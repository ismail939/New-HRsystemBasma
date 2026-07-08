using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HRsystem.Models
{
    public class EmployeeSalary
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(HREmployee))]
        public int EmployeeId { get; set; }

        [JsonIgnore]
        public virtual HREmployee HREmployee { get; set; }

        [Required]
        [ForeignKey(nameof(SalaryComponent))]
        public int SalaryComponentId { get; set; }

        public virtual SalaryComponent SalaryComponent { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime EffectiveDate { get; set; } = DateTime.Now;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}