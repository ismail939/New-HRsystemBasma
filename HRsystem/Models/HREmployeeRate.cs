using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace HRsystem.Models;

public class HREmployeeRate
{
    [Key]
    public int Id { get; set; }
    [Required]
    [ForeignKey(nameof(HREmployee))] // ← tells EF that EmployeeId is the FK for HREmployee
    public int EmployeeId { get; set; }
    [Required]
    public int Month {get; set;}
    [Required]
    public int Year {get; set;}
    [Required]
    public decimal Rate { get; set; }
    public virtual HREmployee HREmployee { get; set; }
}