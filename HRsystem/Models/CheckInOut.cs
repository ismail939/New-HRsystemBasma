using System.ComponentModel.DataAnnotations;
namespace HRsystem.Models;

public class CheckInOut
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId {get; set;}

    [Required] 
    public DateTime CheckTime {get; set;}
}
