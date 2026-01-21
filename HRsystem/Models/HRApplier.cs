using System.ComponentModel.DataAnnotations;
namespace HRsystem.Models;

public class HRApplier
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name {get; set;}

    [Required] // rest of files are in HRApplierFiles
    public bool Status {get; set;}
}
