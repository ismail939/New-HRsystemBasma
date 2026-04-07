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
    public int? LateMinutes { get; set; }
    public int? EarlyLeaveMinutes {get; set;}
    public int? OvertimeMinutes { get; set; }
    public bool Ok {get; set;}
    public int Status { get; set; }
    public string? Notes { get; set; }
}