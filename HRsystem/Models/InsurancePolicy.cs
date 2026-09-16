using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRsystem.Models;

public class InsurancePolicy
{
    [Key] public int Id { get; set; }
    [Required, ForeignKey(nameof(PayrollPolicy))] public int PayrollPolicyId { get; set; }
    public virtual PayrollPolicy PayrollPolicy { get; set; } = null!;
    [Required, StringLength(100)] public string Name { get; set; } = string.Empty;
    public decimal EmployeeRate { get; set; }
    public decimal EmployerRate { get; set; }
    public decimal? MinimumInsurableSalary { get; set; }
    public decimal? MaximumInsurableSalary { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime EffectiveDate { get; set; } = DateTime.Today;
}
