using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
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
    public float? TotalHours { get; set; } 
    public float? LateMinutes { get; set; }
    public float? EarlyLeaveMinutes { get; set; }
    public float? OvertimeMinutes { get; set; }    
    public int Status { get; set; } = 1; // 1: arrived, 0: absent, 2: on leave, 3: absent|on leave.
    public string? OffDayType { get; set; } // "sick", "vacation", "other"
    public string? Notes { get; set; }
    [Required]
    [JsonIgnore]
    public virtual HREmployee HREmployee { get; set; }
}