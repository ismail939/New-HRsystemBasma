using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace HRsystem.Models;

public class ShiftOverride
{
    [Key]
    public int Id { get; set; }

    public int? RequiredHours { get; set; }

    public DateTime? StartTime { get; set; }


    public DateTime? EndTime { get; set; }

    /// <summary>0=Sunday, 1=Monday, ... 6=Saturday</summary>
    [Required]
    public int DayOfWeek { get; set; }    

    [ForeignKey(nameof(HREmployeeShift))]
    public int? HREmployeeShiftId { get; set; }
    [JsonIgnore]
    public virtual HREmployeeShift HREmployeeShift { get; set; }

}