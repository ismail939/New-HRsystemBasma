using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
namespace HRsystem.Models;

[Index(nameof(HREmployeeId), nameof(FromDate))]
public class HREmployeeShift
{
    [Key]
    public int Id { get; set; }

    [Required]
    public DateTime FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public int? LateToleranceMinutes { get; set; }
    public int? EarlyLeaveToleranceMinutes { get; set; }

    // ✅ FK الجديد
    [ForeignKey(nameof(HRShiftOption))]
    public int? ShiftOptionId { get; set; }

    public virtual HRShiftOption HRShiftOption { get; set; }

    // Employee
    [ForeignKey(nameof(HREmployee))]
    public int HREmployeeId { get; set; }
    [JsonIgnore]
    public virtual HREmployee HREmployee { get; set; }
}