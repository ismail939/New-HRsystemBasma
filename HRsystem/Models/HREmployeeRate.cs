using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
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
    [Precision(18, 4)] public decimal Rate { get; set; }
    [JsonIgnore]
    public virtual HREmployee HREmployee { get; set; }
}
