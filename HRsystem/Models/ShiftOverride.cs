using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
namespace HRsystem.Models;


[Index(nameof(HREmployeeShiftId), nameof(DayOfWeek), IsUnique = true)]
public class ShiftOverride
{
    [Key]
    public int Id { get; set; }

    /// <summary>0=Sunday, 1=Monday, ... 6=Saturday</summary>
    [Required]
    public int DayOfWeek { get; set; }    
     // 👇 FK للشيفت الجديد (override)
    [ForeignKey(nameof(HRShiftOption))]
    public int ShiftOptionId { get; set; }
    [JsonIgnore]
    public virtual HRShiftOption HRShiftOption { get; set; }
    [ForeignKey(nameof(HREmployeeShift))]
    public int HREmployeeShiftId { get; set; }
    [JsonIgnore]
    public virtual HREmployeeShift HREmployeeShift { get; set; }

}