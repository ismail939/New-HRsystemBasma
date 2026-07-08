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

    public string? NationalId { get; set; }

   
    [Phone]
    public string? PhoneNumber { get; set; }
    
    public string? MarriageStatus { get; set; }
    
    public string? Religion { get; set; }
    
    public DateTime? DateOfBirth { get; set; }
    
    public string? InsuranceNumber { get; set; }
    public string? Address { get; set; }
    [Required]
    public DateTime HireDate { get; set; }
    public DateTime? EndDate { get; set; }
    
    [Required]
    public string JobName { get; set; }
 
    public string? ContractType { get; set; }
    public string? LeaveReason {get; set;}
    public int? BasmaId { get; set; }
    [ForeignKey("HRDepartment")]
    public int? HRDepartmentId { get; set; }   // Foreign Key
    [JsonIgnore]
    public virtual HRDepartment? HRDepartment { get; set; }

    // Payroll navigation
    [JsonIgnore]
    public virtual ICollection<EmployeeSalary>? EmployeeSalaries { get; set; }
}
