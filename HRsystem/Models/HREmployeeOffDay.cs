using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace HRsystem.Models;

public class HREmployeeOffDay
{
    [Key]
    public int Id { get; set; }
    [Required]
    [ForeignKey(nameof(HREmployee))] // ← tells EF that EmployeeId is the FK for HREmployee
    public int EmployeeId { get; set; }
    [Required]
    public DateTime OffDayDate { get; set; }
    public string? OffDayType { get; set; }
    [Required]
    [JsonIgnore]
    public virtual HREmployee HREmployee { get; set; }
}
