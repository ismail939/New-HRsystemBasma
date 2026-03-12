namespace HRsystem.ViewModels{
public class EmployeeViewModel
{
    public int? Id { get; set; }

    
    public string? Department { get; set; }
    
    public string? Name { get; set; }

    
    public string? NationalId { get; set; }

   
    public string? PhoneNumber { get; set; }
   
    public string? MarriageStatus { get; set; }
 
    public string? Religion { get; set; }
    public DateTime DateOfBirth { get; set; }
    
    public string? InsuranceNumber { get; set; }
    
    public DateTime HireDate { get; set; }
    public DateTime? EndDate { get; set; }
    
    
    public string? JobName { get; set; }
  
    public string? ContractType { get; set; }
    public string? LeaveReason {get; set;}
    public int? BasmaId { get; set; }
    public int? HRDepartmentId { get; set; }   // Foreign Key

}
}