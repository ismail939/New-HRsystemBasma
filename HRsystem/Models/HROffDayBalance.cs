using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace HRsystem.Models;

public class HROffDayBalance
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int Annual { get; set; }
    [Required]
    public int Casual { get; set; }
    [Required]
    public int Off { get; set; }
    [Required]
    public int CompensatoryOfNationalHoliday { get; set; }
    [Required]
    public string Notes { get; set; } = string.Empty;
    [ForeignKey(nameof(HREmployee))]
    public int? EmployeeId { get; set; }
    [JsonIgnore]
    public virtual HREmployee HREmployee { get; set; }
}

