using HRsystem.Models;
using HRsystem.ViewModels;

namespace HRsystem.ViewModels{
public class ListEmployeesViewModel
{
    public List<EmployeeViewModel> Employees {get; set;}
    public List<HRDepartment> Departments {get; set;}
}
}