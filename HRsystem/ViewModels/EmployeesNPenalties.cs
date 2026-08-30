using HRsystem.Models;
namespace HRsystem.ViewModels;
public class EmployeesNPenalties
{
    public List<SimpleEmployeeViewModel> Employees { get; set; }           // your existing model
    public List<object> Penalties { get; set; } = new();                    // penalty DTOs (fetched per employee client-side)
}
