using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
namespace HRsystem.Models;

[Index(nameof(UserId), nameof(CheckTime))]
public class CheckInOut
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId {get; set;}

    [Required] 
    public DateTime CheckTime {get; set;}
}
