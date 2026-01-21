public class EditOffDaysRequest
{
    public int EmployeeId { get; set; }
    public List<OffDayDto> Days { get; set; }
}

public class OffDayDto
{
    public string OffDayDate { get; set; }
    public bool IsOffDay { get; set; }
    public string OffDayType { get; set; }
}
