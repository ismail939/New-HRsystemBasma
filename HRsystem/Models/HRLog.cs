using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace HRsystem.Models;

public class HRLog
{
    [Key]
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    [Required]
    public string Action { get; set; } = string.Empty;
}