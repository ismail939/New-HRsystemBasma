using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

public class DepartmentPostRequest
{
    public string Name {get; set;}
    public string Code {get; set;}
    public string? Description { get; set; }
     public int? ParentDepartmentId { get; set; }
     public int? ManagerId {get; set;}
      // Add these:
    public List<SelectListItem> Departments { get; set; } = new();
    public List<SelectListItem> Managers { get; set; } = new();
}