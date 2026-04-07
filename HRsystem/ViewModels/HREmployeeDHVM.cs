using System;
using System.Collections.Generic;
using HRsystem.ViewModels;

namespace HRsystem.Models
{
    // Details for a leave (Agaza)
    public class LeaveDetail
    {
        public DateTime Date { get; set; }
        public string Type { get; set; } // e.g., "Annual", "Sick"
    }

    

    // Details for an absence (Geza2a)
    public class PenaltyDetail
    {
        public DateTime Date { get; set; }
        public string Decision { get; set; }
        public string Reason { get; set; }
    }

    // Details for 8yab (lateness / check-in?)
    public class AbsenceDetail
    {
        public DateTime Date { get; set; }
    }

// ViewModel for report
public class HREmployeeDHVM
{
    public string DepartmentName { get; set; }
    public DateTime ReportStartDate { get; set; }
    public DateTime ReportEndDate { get; set; }
    public string EmployeeName { get; set; }
    public decimal TotalRate { get; set; }

    // Entry days count
    public int EntryDaysCount { get; set; }

    // Leaves (Agazat)
    public List<LeaveDetail> Leaves { get; set; } = new List<LeaveDetail>();
    public List<LeaveDetail> Offs {get;set;} = new List<LeaveDetail>();
    public List<LeaveDetail> Ills {get;set;} = new List<LeaveDetail>();
    public int LeavesCount => Leaves?.Count ?? 0;
    public int OffsCount => Offs?.Count ?? 0;
    public int IllsCount => Ills?.Count ?? 0; 
    public float TotalWorkHours { get; set; }
    public int TotalLateMinutes { get; set; }
    public int TotalEarlyLeaveMinutes { get; set; }
    public int TotalOvertimeMinutes { get; set; }
    // Absences (Geza2at)
    public List<AbsenceDetail> Absences { get; set; } = new List<AbsenceDetail>();
    public int AbsencesCount => Absences?.Count ?? 0;

    // 8yab
    public List<PenaltyDetail> Penalty { get; set; } = new List<PenaltyDetail > ();
    public int PenaltyCount => Penalty?.Count ?? 0;
    public List<BasmaWithShiftVM> BasmaList { get; set; } = new List<BasmaWithShiftVM>();
    }
}
