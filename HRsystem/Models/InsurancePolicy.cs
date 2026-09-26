using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRsystem.Models;

public class InsurancePolicy
{
    [Key] public int Id { get; set; }
    [Required, ForeignKey(nameof(PayrollPolicy))] public int PayrollPolicyId { get; set; }
    public virtual PayrollPolicy PayrollPolicy { get; set; } = null!;
    [Required, StringLength(100)] public string Name { get; set; } = string.Empty;
    [Precision(18, 4)] public decimal EmployeeRate { get; set; }
    [Precision(18, 4)] public decimal EmployerRate { get; set; }
    [Precision(18, 2)] public decimal? MinimumInsurableSalary { get; set; }
    [Precision(18, 2)] public decimal? MaximumInsurableSalary { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime EffectiveDate { get; set; } = DateTime.Today;
}
