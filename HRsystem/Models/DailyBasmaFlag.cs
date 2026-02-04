using System.ComponentModel.DataAnnotations;
namespace HRsystem.Models;

public class DailyBasmaFlag
{
    [Required]
    public bool Taken {get; set;} = false;

    [Key]
    [Required] 
    public DateTime Day {get; set;}
}
