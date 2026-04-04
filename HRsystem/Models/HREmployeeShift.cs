using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace HRsystem.Models;

public class HREmployeeShift
{
    [Key]
    public int Id { get; set; }

    public int? RequiredHours { get; set; }

    public DateTime? StartTime { get; set; }


    public DateTime? EndTime { get; set; }


    [Required]
    public int ShiftMode { get; set; } = 0; // 0=variable, 1=hours, 2=fixed.

    [Required]
    public DateTime FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public int? LateToleranceMinutes { get; set; }

    public int? EarlyLeaveToleranceMinutes { get; set; }

    [ForeignKey(nameof(HREmployee))]
    public int? EmployeeId { get; set; }
    [JsonIgnore]
    public virtual HREmployee HREmployee { get; set; }

}