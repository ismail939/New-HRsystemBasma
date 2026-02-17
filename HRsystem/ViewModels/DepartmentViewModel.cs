public class DepartmentViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }

    public int? ParentDepartmentId { get; set; }
    public string ParentDepartmentName { get; set; }

    public int? ManagerId { get; set; }
    public string ManagerName { get; set; }
}
