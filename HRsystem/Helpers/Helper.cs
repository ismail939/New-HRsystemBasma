using HRsystem.Models;
namespace HRsystem.Controllers
{
public class Helper
{
            public class LatencyResult
        {
            public int LateMinutes { get; set; }
            public int EarlyLeaveMinutes { get; set; }
            public int OvertimeMinutes { get; set; }
            public float TotalHours { get; set; }
        }
     public LatencyResult LatencyForEmployee(List<HREmployeeShift> employeeShifts, int employeeId, DateTime checkIn, DateTime checkOut)
        {
            var shift = employeeShifts.FirstOrDefault(es => es.HREmployeeId == employeeId);
            if (shift == null)
            {
                return new LatencyResult
                {
                    LateMinutes = 0,
                    EarlyLeaveMinutes = 0,
                    OvertimeMinutes = 0,
                    TotalHours = 0
                };
            }

            // if(shift.ShiftMode == 0)
            // {
            //     return new LatencyResult
            //     {
            //         LateMinutes = 0,
            //         EarlyLeaveMinutes = 0,
            //         OvertimeMinutes = 0,

            //         TotalHours = checkOut.Subtract(checkIn).Hours
            //     };
            // }else if(shift.ShiftMode == 1)
            // {
            //     return new LatencyResult
            //     {
            //         LateMinutes = 0,
            //         EarlyLeaveMinutes = 0,
            //         OvertimeMinutes = checkOut.Subtract(checkIn).Minutes > 0 ? (int)checkOut.Subtract(checkIn).TotalMinutes - (int)(shift.EndTime?.Subtract(shift.StartTime ?? checkIn).TotalMinutes ?? 0) : 0,
            //         TotalHours = checkOut.Subtract(checkIn).Hours
            //     };
            // }
            // else if(shift.ShiftMode == 2)
            // {
            //     return new LatencyResult
            //     {
            //         // LateMinutes = checkIn.Subtract(shift.StartTime ?? checkIn).Minutes,
            //         // EarlyLeaveMinutes = (shift.EndTime ?? checkOut).Subtract(checkOut).Minutes,
            //         // OvertimeMinutes = checkOut.Subtract(checkIn).Minutes > 0 ? (int)checkOut.Subtract(checkIn).TotalMinutes - (int)(shift.EndTime?.Subtract(shift.StartTime ?? checkIn).TotalMinutes ?? 0) : 0,
            //         TotalHours = checkOut.Subtract(checkIn).Hours
            //     };
            // }
            return new LatencyResult
            {
                LateMinutes = 0,
                EarlyLeaveMinutes = 0,
                OvertimeMinutes = 0,
                TotalHours = 0
            };
        }
}
}