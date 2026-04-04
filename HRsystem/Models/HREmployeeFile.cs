using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace HRsystem.Models;

public class HREmployeeFile
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    [Required]
    [StringLength(100)]
    public string FileName { get; set; }

    [Required]
    public string Url { get; set; }
    [JsonIgnore]
    public virtual HREmployee Employee { get; set; }
}
