using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace HRsystem.Models;

public class HREmployeeBasma
{
    [Key]
    public int Id { get; set; }
    [Required]
    [ForeignKey(nameof(HREmployee))] // ← tells EF that EmployeeId is the FK for HREmployee
    public int EmployeeId { get; set; }
    [Required]
    public DateTime DayDate { get; set; }
    public DateTime? ArrivalTime { get; set; }
    public DateTime? DepartureTime { get; set; }
    public int? TotalHours { get; set; } 
    public int? LateMinutes { get; set; }
    public string? Status { get; set; }
    public string? Notes { get; set; }
    [Required]
    public virtual HREmployee HREmployee { get; set; }
}