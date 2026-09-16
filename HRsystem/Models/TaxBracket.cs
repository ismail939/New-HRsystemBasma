using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRsystem.Models;

public class TaxBracket
{
    [Key] public int Id { get; set; }
    [Required, ForeignKey(nameof(PayrollPolicy))] public int PayrollPolicyId { get; set; }
    public virtual PayrollPolicy PayrollPolicy { get; set; } = null!;
    [Required] public decimal FromAmount { get; set; }
    public decimal? ToAmount { get; set; }
    [Required] public decimal Rate { get; set; }
    public decimal FixedDeduction { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime EffectiveDate { get; set; } = DateTime.Today;
}
