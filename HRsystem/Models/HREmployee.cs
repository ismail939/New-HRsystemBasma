using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
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
    
    public string? InsuranceNumber { get; set; }
    [Required]
    public DateTime HireDate { get; set; }
    public DateTime? EndDate { get; set; }
    
    [Required]
    public string JobName { get; set; }
    [Required]
    public string ContractType { get; set; }
    public string? LeaveReason {get; set;}
    public int? BasmaId { get; set; }
    [ForeignKey("HRDepartment")]
    public int? HRDepartmentId { get; set; }   // Foreign Key
    [JsonIgnore]
    public virtual HRDepartment? HRDepartment { get; set; }
}