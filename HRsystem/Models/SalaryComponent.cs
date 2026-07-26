using System.ComponentModel.DataAnnotations;

namespace HRsystem.Models
{
    public class SalaryComponent
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string NameAr { get; set; } = string.Empty;

        /// <summary>Earning or Deduction</summary>
        [Required]
        [StringLength(20)]
        public string Type { get; set; } = "Earning"; // Earning / Deduction

        /// <summary>FixedAmount or Percentage</summary>
        [Required]
        [StringLength(20)]
        public string CalculationMethod { get; set; } = "FixedAmount"; // FixedAmount / Percentage

        public decimal? DefaultAmount { get; set; }

        public bool IsActive { get; set; } = true;

        /// <summary>هل هذا المكون يخضع للضريبة (Tax)؟</summary>
        public bool IsTaxable { get; set; } = true;

        /// <summary>هل هذا المكون يخضع للتأمينات الاجتماعية؟</summary>
        public bool IsInsurable { get; set; } = true;

        /// <summary>هل هذا المكون ثابت (يتكرر كل شهر)؟</summary>
        public bool IsFixed { get; set; } = true;

        [StringLength(500)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}