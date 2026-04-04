using HRsystem.Models;
namespace HRsystem.ViewModels;
public class EmployeesNPenalties
{
    public List<SimpleEmployeeViewModel> Employees { get; set; }           // your existing model
    public List<HREmployeePenalty> Penalties { get; set; }  // the list you want to display
}
