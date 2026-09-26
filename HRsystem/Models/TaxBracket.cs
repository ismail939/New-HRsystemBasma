using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HRsystem.Models;

public class TaxBracket
{
    [Key] public int Id { get; set; }
    [Required, ForeignKey(nameof(PayrollPolicy))] public int PayrollPolicyId { get; set; }
    public virtual PayrollPolicy PayrollPolicy { get; set; } = null!;
    [Required, Precision(18, 2)] public decimal FromAmount { get; set; }
    [Precision(18, 2)] public decimal? ToAmount { get; set; }
    [Required, Precision(18, 4)] public decimal Rate { get; set; }
    [Precision(18, 2)] public decimal FixedDeduction { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime EffectiveDate { get; set; } = DateTime.Today;
}
