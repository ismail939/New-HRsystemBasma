using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace HRsystem.Models;

public class HRDepartment
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [StringLength(10)]
    public string Code { get; set; }   // e.g., HR, IT, FIN

    [StringLength(250)]
    public string? Description { get; set; }

    
    // Parent Department (for hierarchy)
    public int? ParentDepartmentId { get; set; }

    [ForeignKey("ParentDepartmentId")]
    public virtual HREmployee ParentDepartment { get; set; }

    public virtual ICollection<HRDepartment> SubDepartments { get; set; }

    // Manager (Employee)
    public int? ManagerId { get; set; }

    [ForeignKey("ManagerId")]
    public virtual HREmployee Manager { get; set; }

    // Employees in Department
    public virtual ICollection<HREmployee> Employees { get; set; }

    // Audit Fields
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
}
