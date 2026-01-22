using System.ComponentModel.DataAnnotations;
namespace HRsystem.Models;

public class HREmployee
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    public string NationalId { get; set; }

    [Required]
    [Phone]
    public string PhoneNumber { get; set; }
    [Required]
    public string MarriageStatus { get; set; }
    [Required]
    public string Religion { get; set; }
    [Required]
    public DateTime DateOfBirth { get; set; }
    [Required]
    public string InsuranceNumber { get; set; }
    [Required]
    public DateTime HireDate { get; set; }
    public DateTime? EndDate { get; set; }
    [Required]
    public string Department { get; set; }
    [Required]
    public string JobName { get; set; }
    [Required]
    public string ContractType { get; set; }
    public string? LeaveReason {get; set;}
}