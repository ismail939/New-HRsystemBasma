using HRsystem.Models;
namespace HRsystem.ViewModels;
public class SimpleEmployeeViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
  
    public string PhoneNumber { get; set; }

    public DateTime HireDate { get; set; }
 
    public string JobName { get; set; }

    public string Department { get; set; } // new property to hold department name
}
