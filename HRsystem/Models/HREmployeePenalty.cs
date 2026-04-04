using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace HRsystem.Models;

public class HREmployeePenalty
{
    [Key]
    public int Id { get; set; }
    [Required]
    [ForeignKey(nameof(HREmployee))] // ← tells EF that EmployeeId is the FK for HREmployee
    public int EmployeeId { get; set; }
    [Required]
    public DateTime PenaltyDate { get; set; }
    [Required]
    public int PenaltyPoints { get; set; }
    [Required]
    public string Decision { get; set; }
    public string? Reason { get; set; }
    public bool IsActive { get; set; } = false;
    [JsonIgnore]
    public virtual HREmployee HREmployee { get; set; }
}