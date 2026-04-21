using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using HRsystem.Models;
using Microsoft.EntityFrameworkCore;


public class HRShiftOption
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }          // "Morning 8–5", "Night shift", etc.

    [Required]
    public int ShiftMode { get; set; }        // 0=variable, 1=hours, 2=fixed

    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? RequiredHours { get; set; }
    public int? LateToleranceMinutes { get; set; }
    public int? EarlyLeaveToleranceMinutes { get; set; }

    public bool IsActive { get; set; } = true;

    [JsonIgnore]
    public virtual ICollection<HREmployeeShift> EmployeeShifts { get; set; } = new List<HREmployeeShift>();
    [JsonIgnore]
    public virtual ICollection<ShiftOverride> ShiftOverrides { get; set; } = new List<ShiftOverride>();
}