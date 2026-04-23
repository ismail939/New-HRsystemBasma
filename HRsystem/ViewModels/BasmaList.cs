using System.ComponentModel.DataAnnotations;
using HRsystem.Models;
namespace HRsystem.ViewModels;
public class BasmaList
{
    public int Id { get; set; }
    public string? EmployeeName{ get; set; }
    public DateTime DayDate { get; set; }
    public DateTime? ArrivalTime { get; set; }
    public DateTime? DepartureTime { get; set; }
    public float? TotalHours { get; set; }
    public float? LateMinutes { get; set; }
    public float? EarlyLeaveMinutes {get; set;}
    public float? OvertimeMinutes { get; set; }
    public int Status { get; set; }
    public string? Notes { get; set; }
    public string? OffDayType { get; set; }
}