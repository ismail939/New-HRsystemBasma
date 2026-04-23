using HRsystem.Data;
using HRsystem.Models;
using HRsystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Windows.Forms;

// using ZkFingerprintBridge;

namespace HRsystem.Controllers
{
    public class BasmaController : Controller
    {
        private readonly ILogger<BasmaController> _logger;
        private readonly AppDbContext _context;
        public BasmaController(ILogger<BasmaController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public class LatencyResult
        {
            public float LateMinutes { get; set; }
            public float EarlyLeaveMinutes { get; set; }
            public float OvertimeMinutes { get; set; }
            public float TotalHours { get; set; }
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpGet]
        [Route("/basma")]
        public IActionResult Basma()
        {
            return View("basma");
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpGet]
        [Route("/basmaData")]
        public IActionResult BasmaData(DateTime Day)
        {
            return RedirectToAction("TakeDayFromFingerPrint", "Basma", new { Day = Day.Date });
        }

        [Authorize(Roles = "Admin,HR")]
        public IActionResult GetDayBasma(DateTime Day)
        {
            Console.WriteLine("🟢 Entered taken");
            var list = _context.HREmployeeBasmas.Where(basma => basma.DayDate.Date == Day.Date).ToList();
            var employees = _context.HREmployees.ToList();
            List<BasmaList> basmaList = [];
            foreach (var basma in list)
            {
                var emp = employees.FirstOrDefault(e => e.Id == basma.EmployeeId);
                if (emp != null)
                {
                    basmaList.Add(new BasmaList
                    {
                        Id = basma.Id,
                        EmployeeName = emp.Name,
                        DayDate = basma.DayDate,
                        ArrivalTime = basma.ArrivalTime,
                        DepartureTime = basma.DepartureTime,
                        TotalHours = basma.TotalHours,
                        LateMinutes = basma.LateMinutes,
                        EarlyLeaveMinutes = basma.EarlyLeaveMinutes,
                        OvertimeMinutes = basma.OvertimeMinutes,
                        OffDayType = basma.OffDayType,
                        Status = basma.Status,
                        Notes = basma.Notes
                    });
                }
            }
            ;
            basmaList = basmaList.OrderBy(b => b.EmployeeName).ToList();
            basmaList.ForEach(p => Console.WriteLine($"🟢 Basma List Entry: {p.EmployeeName}, {p.DayDate}, {p.ArrivalTime}, {p.DepartureTime}, {p.TotalHours}, {p.LateMinutes}, {p.EarlyLeaveMinutes}, {p.OvertimeMinutes}, {p.Status}, {p.Notes}"));
            return Json(basmaList);
        }

        [HttpPost]
        [Route("/cancelBasma")]
        public IActionResult ToggleBasmaStatus(int id)
        {
            var basmaEntry = _context.HREmployeeBasmas.FirstOrDefault(b => b.Id == id);
            if (basmaEntry == null)
            {
                return NotFound("Basma entry not found.");
            }
            basmaEntry.Status = 3; // either offday or absent, HR will decide
            _context.HRLogs.Add(new HRLog
            {
                Action = $"User ({User.Identity.Name}) canceled basma on day  ({basmaEntry.DayDate.ToShortDateString()}) for employee ID ({basmaEntry.EmployeeId})"
            });

            _context.SaveChanges();
            return Ok(new { success = true });
        }


        [Authorize(Roles = "Admin,HR")]
        public IActionResult TakeDayFromFingerPrint(DateTime Day)
        {
            if (Day.Date > DateTime.Now.Date)
            {
                return RedirectToAction("GetDayBasma", "Basma", new { Day = Day.Date });
            }

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                Console.WriteLine("🟢 Start Sync Basma");

                var dayDate = Day.Date;

                // 🟢 SHIFT OPTIONS
                var shiftOptionsDict = _context.HRShiftOptions
                    .AsNoTracking()
                    .ToDictionary(x => x.Id);

                // 🟢 EMPLOYEE SHIFTS
                var employeeShifts = _context.HREmployeeShift
                    .AsNoTracking()
                    .Where(s =>
                        s.FromDate <= dayDate &&
                        (s.ToDate == null || s.ToDate >= dayDate))
                    .ToList();

                var shiftMap = employeeShifts
                    .GroupBy(s => s.HREmployeeId)
                    .ToDictionary(g => g.Key, g => g.First());

                var shiftIds = employeeShifts.Select(s => s.Id).ToHashSet();

                // 🟢 OVERRIDES → optimized lookup
                var overridesLookup = _context.ShiftOverrides
                    .AsNoTracking()
                    .Where(o => shiftIds.Contains(o.HREmployeeShiftId))
                    .ToLookup(o => (o.HREmployeeShiftId, o.DayOfWeek));

                // 🟢 EMPLOYEES
                var employees = _context.HREmployees
                .AsNoTracking()
                .Where(e => e.BasmaId != null)
                .ToLookup(e => e.BasmaId);
                // 🟢 BASMAS
                var basmas = _context.HREmployeeBasmas
                    .Where(b => b.DayDate == dayDate)
                    .ToList();

                var basmaDict = basmas.ToDictionary(b => b.EmployeeId);

                // 🟢 OFF DAYS
                var offDays = _context.HREmployeeOffDays
                    .AsNoTracking()
                    .Where(o => o.OffDayDate == dayDate)
                    .ToDictionary(o => o.EmployeeId, o => o.OffDayType);

                // 🟢 CHECKINS
                var start = dayDate.AddHours(3);
                var end = dayDate.AddDays(1).AddHours(3);

                var checkGroups = _context.CheckInOuts
                    .AsNoTracking()
                    .Where(c => c.CheckTime >= start && c.CheckTime <= end)
                    .GroupBy(c => c.UserId)
                    .ToList();

                // 🟢 PROCESS CHECKINS
                foreach (var group in checkGroups)
                {
                    var employee = employees[group.Key].FirstOrDefault();
                    if (employee == null) continue;

                    if (!shiftMap.TryGetValue(employee.Id, out var shift))
                        continue;

                    int dayOfWeek = (int)dayDate.DayOfWeek;

                    var overrideShift = overridesLookup[(shift.Id, dayOfWeek)].FirstOrDefault();

                    int shiftOptionId = overrideShift?.ShiftOptionId
                        ?? shift.ShiftOptionId
                        ?? 0;

                    if (!shiftOptionsDict.TryGetValue(shiftOptionId, out var shiftOption))
                        continue;

                    DateTime arrival = DateTime.MaxValue;
                    DateTime departure = DateTime.MinValue;
                    int count = 0;
                    Console.WriteLine($"🟢❤️❤️❤️❤️ Processing CheckInOut for EmployeeId {employee.Id} with ShiftOptionId {shiftOptionId}");
                    foreach (var t in group)
                    {
                        Console.WriteLine($"🟢 CheckInOut for UserId {t.UserId}: {t.CheckTime}");
                        if (t.CheckTime < arrival) arrival = t.CheckTime;
                        if (t.CheckTime > departure) departure = t.CheckTime;
                        count++;
                    }

                    if (!basmaDict.TryGetValue(employee.Id, out var basma))
                    {
                        basma = new HREmployeeBasma
                        {
                            EmployeeId = employee.Id,
                            DayDate = dayDate
                        };

                        _context.HREmployeeBasmas.Add(basma);
                        basmaDict[employee.Id] = basma;
                    }

                    if (count == 1 || departure.Subtract(arrival).TotalMinutes < 10) // if only one record or the time difference is less than 10 minutes, consider it as a single check-in or check-out
                    {
                        basma.ArrivalTime = arrival;
                        basma.DepartureTime = departure;
                        basma.LateMinutes = 0;
                        basma.EarlyLeaveMinutes = 0;
                        basma.OvertimeMinutes = 0;
                        basma.TotalHours = 0;
                        basma.Status = 1;
                        continue;
                    }

                    var shiftStart = dayDate.Add(shiftOption.StartTime.Value.TimeOfDay);
                    var shiftEnd = dayDate.Add(shiftOption.EndTime.Value.TimeOfDay);

                    if (shiftOption.EndTime < shiftOption.StartTime)
                        shiftEnd = shiftEnd.AddDays(1);

                    var allowedStart = shiftStart.AddMinutes(shift.LateToleranceMinutes ?? 0);
                    var allowedEnd = shiftEnd.AddMinutes(-(shift.EarlyLeaveToleranceMinutes ?? 0));

                    float late = arrival > allowedStart
                        ? (float)(arrival - allowedStart).TotalMinutes / 60f
                        : 0;

                    float early = departure < allowedEnd
                        ? (float)(allowedEnd - departure).TotalMinutes / 60f
                        : 0;

                    float overtime = departure > shiftEnd
                        ? (float)(departure - shiftEnd).TotalMinutes / 60f
                        : 0;

                    float total = (float)(departure - arrival).TotalMinutes / 60f;

                    basma.ArrivalTime = arrival;
                    basma.DepartureTime = departure;
                    basma.LateMinutes = late;
                    basma.EarlyLeaveMinutes = early;
                    basma.OvertimeMinutes = overtime;
                    basma.TotalHours = total;
                    basma.Status = 1;
                }

                // 🟢 OFF / ABSENT
                foreach (var emp in employees.SelectMany(g => g))
                {
                    int empId = emp.Id;

                    if (basmaDict.TryGetValue(empId, out var existing) && existing.Status == 1)
                        continue;

                    if (!basmaDict.TryGetValue(empId, out var basma))
                    {
                        basma = new HREmployeeBasma
                        {
                            EmployeeId = empId,
                            DayDate = dayDate
                        };

                        _context.HREmployeeBasmas.Add(basma);
                        basmaDict[empId] = basma;
                    }

                    bool isOff = offDays.TryGetValue(empId, out var offType);

                    basma.ArrivalTime = null;
                    basma.DepartureTime = null;
                    basma.Status = isOff ? 2 : 0;
                    basma.OffDayType = isOff ? offType : null;
                }

                _context.SaveChanges();
                transaction.Commit();

                Console.WriteLine("🟢 Basma Sync Completed");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"🔴 Error: {ex}");
            }

            return RedirectToAction("GetDayBasma", new { Day = Day.Date });
        }
        private string FormatHours(float? hours)
        {
            if (hours == null) return "";

            int totalMinutes = (int)(hours.Value * 60);
            int h = totalMinutes / 60;
            int m = totalMinutes % 60;

            return $"{h}:{m:D2}";
        }
        HRShiftOption GetShiftForEmployee(int employeeId, DateTime day)
        {
            var empShift = _context.HREmployeeShift
                .Where(s =>
                    s.HREmployeeId == employeeId &&
                    s.FromDate.Date <= day.Date &&
                    (s.ToDate == null || s.ToDate.Value.Date >= day.Date)
                )
                .FirstOrDefault();

            if (empShift == null)
                return null;

            var overrideShift = _context.ShiftOverrides
                .FirstOrDefault(o =>
                    o.HREmployeeShiftId == empShift.Id &&
                    o.DayOfWeek == (int)day.DayOfWeek
                );

            if (overrideShift != null)
                return overrideShift.HRShiftOption;

            return empShift.HRShiftOption;
        }
        [HttpGet]
        [Route("/dailyReporttt")]
        public IActionResult DailyReporttt(DateTime Day)
        {
            var basmas = _context.HREmployeeBasmas
                .Where(b => b.DayDate.Date == Day.Date)
                .ToList();

            var employees = _context.HREmployees
                .ToDictionary(e => e.Id);
            var employeeShifts = _context.HREmployeeShift
            .Include(s => s.HRShiftOption)
            .Include(s => s.HREmployee)
            .ToList();

            var shiftOverrides = _context.ShiftOverrides
                .Include(o => o.HRShiftOption)
                .ToList();
            // ✅ departments preload
            var departments = _context.HRDepartments
                .ToDictionary(d => d.Id, d => d.Name);

            var model = new DailyReportVM
            {
                Day = Day
            };

            foreach (var b in basmas)
            {
                if (!employees.TryGetValue(b.EmployeeId, out var emp))
                    continue;

                string statusText;

                bool isOffDay = false;

                if (b.Status == 1)
                {
                    statusText = "حضور";
                }
                else if (b.Status == 2)
                {
                    statusText = !string.IsNullOrEmpty(b.OffDayType)
                        ? b.OffDayType
                        : "إجازة";

                    isOffDay = true; // ✅ مهم
                }
                else
                {
                    statusText = "غياب";

                    // لو الغياب عندك يعتبر Off يوم
                    // لو لا احذف السطر ده
                    isOffDay = false;
                }

                HRShiftOption shift = null;

                if (!isOffDay)
                {
                    shift = GetShiftForEmployee(emp.Id, Day);
                }

                model.Rows.Add(new DailyReportRowVM
                {
                    EmployeeName = emp.Name,

                    DepartmentName = emp.HRDepartmentId != null && departments.ContainsKey(emp.HRDepartmentId.Value)
                        ? departments[emp.HRDepartmentId.Value]
                        : "",

                    JobName = emp.JobName ?? "",

                    // ✅ الشرط الجديد
                    ShiftStart = shift?.StartTime?.ToString("hh:mm tt"),
                    ShiftEnd = shift?.EndTime?.ToString("hh:mm tt"),

                    ArrivalTime = b.ArrivalTime?.ToString("hh:mm tt"),
                    DepartureTime = b.DepartureTime?.ToString("hh:mm tt"),

                    TotalHours = FormatHours(b.TotalHours),
                    LateMinutes = FormatHours(b.LateMinutes),
                    EarlyLeaveMinutes = FormatHours(b.EarlyLeaveMinutes),
                    OvertimeMinutes = FormatHours(b.OvertimeMinutes),

                    Status = statusText,
                    Notes = b.Notes
                });
            }
            var report = new DailyReport(model);

            // 4) Generate PDF 
            var pdfBytes = report.GeneratePdf();

            // 5) Return PDF
            return File(pdfBytes, "application/pdf", $"DailyReport-{Day:yyyy-MM-dd}.pdf");
        }

        // [Authorize(Roles = "Admin,HR")]
        // private LatencyResult LatencyForEmployee(List<HREmployeeShift> employeeShifts, int employeeId, DateTime checkIn, DateTime checkOut)
        // {
        //     var shift = employeeShifts.FirstOrDefault(es => es.EmployeeId == employeeId);
        //     if (shift == null)
        //     {
        //         return new LatencyResult
        //         {
        //             LateMinutes = 0,
        //             EarlyLeaveMinutes = 0,
        //             OvertimeMinutes = 0,
        //             TotalHours = (float)checkOut.Subtract(checkIn).TotalMinutes/60.0f
        //         };
        //     }

        //     if (shift.ShiftMode == 0)
        //     {
        //         return new LatencyResult
        //         {
        //             LateMinutes = 0,
        //             EarlyLeaveMinutes = 0,
        //             OvertimeMinutes = 0,
        //             TotalHours = (float)checkOut.Subtract(checkIn).TotalMinutes/60.0f
        //         };
        //     }
        //     else if (shift.ShiftMode == 1)
        //     {
        //         double workedHours = Math.Round((checkOut - checkIn).TotalMinutes / 60.0, 1);

        //         var requiredTimeSpan = TimeSpan.FromHours(shift.RequiredHours ?? 0);
        //         double missingHours = Math.Round(
        //             Math.Max(0, (requiredTimeSpan - (checkOut - checkIn)).TotalMinutes) / 60.0,
        //             1
        //         );
        //         Console.WriteLine("🟢🟢🟢🟢🟢🟢🟢🟢🟢🟢");
        //         Console.WriteLine("🟢" + checkOut.Subtract(checkIn).TotalMinutes / 60.0f);
        //         return new LatencyResult
        //         {
        //             LateMinutes = 0,
        //             EarlyLeaveMinutes = (int)requiredTimeSpan.Subtract(checkOut.Subtract(checkIn)).TotalMinutes,
        //             OvertimeMinutes = (int)Math.Max(0, (checkOut - checkIn - requiredTimeSpan).TotalMinutes),
        //             TotalHours = (float)checkOut.Subtract(checkIn).TotalMinutes / 60.0f
        //         };
        //     }
        //     else if (shift.ShiftMode == 2)
        //     {
        //         double workedHours = Math.Round((checkOut - checkIn).TotalMinutes / 60.0, 1);

        //         var requiredTimeSpan = TimeSpan.FromHours(shift.RequiredHours ?? 0);
        //         double missingHours = Math.Round(
        //             Math.Max(0, (requiredTimeSpan - (checkOut - checkIn)).TotalMinutes) / 60.0,
        //             1
        //         );
        //         var checkInTime = checkIn.TimeOfDay;
        //         var shiftStartTime = (shift.StartTime ?? checkIn).TimeOfDay;

        //         var lateMinutes = (int)Math.Max(
        //             0,
        //             (checkInTime - shiftStartTime).TotalMinutes
        //         );
        //         var earlyLeaveMinutes = (int)Math.Max(
        //             0,
        //             ((shift.EndTime ?? checkOut).TimeOfDay - checkOut.TimeOfDay).TotalMinutes
        //         );
        //         var overtimeMinutes = (int)Math.Max(
        //             0,
        //             (checkOut - checkIn - requiredTimeSpan).TotalMinutes
        //         );
        //         Console.WriteLine("🟢🟢🟢🟢🟢🟢");
        //         Console.WriteLine("🟢" + checkOut.Subtract(checkIn).TotalMinutes / 60.0f);
        //         return new LatencyResult
        //         {
        //             LateMinutes = lateMinutes,
        //             EarlyLeaveMinutes = earlyLeaveMinutes,
        //             OvertimeMinutes = overtimeMinutes,
        //             TotalHours = (float)checkOut.Subtract(checkIn).TotalMinutes / 60.0f
        //         };
        //     }
        //     return new LatencyResult
        //     {
        //         LateMinutes = 0,
        //         EarlyLeaveMinutes = 0,
        //         OvertimeMinutes = 0,
        //         TotalHours = 0
        //     };
        // }


        // [Authorize(Roles = "Admin,HR")]
        // [HttpPost]
        // [Route("/confirmBasma")]
        // public IActionResult ConfirmBasma(int id, int type)
        // {
        //     var basmaEntry = _context.HREmployeeBasmas.FirstOrDefault(b => b.Id == id);
        //     if (basmaEntry != null)
        //     {
        //         basmaEntry.Ok = true;
        //         if (type == 2)
        //         {
        //             // ask whether there is an offday
        //             var isOffDay = _context.HREmployeeOffDays
        //                 .Any(od => od.EmployeeId == basmaEntry.EmployeeId && od.OffDayDate.Date == basmaEntry.DayDate.Date);
        //             Console.WriteLine($"🟢 Checking off day for EmployeeId {basmaEntry.Id} on {basmaEntry.DayDate.Date}: {isOffDay}");
        //             if (isOffDay)
        //             {
        //                 basmaEntry.Status = 2; // offday
        //                 _context.SaveChanges();
        //                 return Json(new { code = 1 });
        //             }
        //             return Json(new { code = 2, message = "No off day found for the employee on that date." });
        //         }
        //         else if (type == 0)
        //         {
        //             basmaEntry.Status = 0; // absent
        //             _context.SaveChanges();
        //             return Json(new { code = 1 });
        //         }
        //         else if (type == 1)
        //         {
        //             basmaEntry.Status = 1; // present
        //             _context.SaveChanges();
        //             return Json(new { code = 1 });
        //         }
        //     }
        //     return Json(new { success = false, message = "Basma entry not found." });
        // }

        [Authorize(Roles = "Admin,HR")]
        [HttpPost]
        [Route("/saveBasmaNotes")]
        public IActionResult SaveBasmaNotes([FromBody] List<BasmaNote> basmaNotes)
        {
            foreach (var basma in basmaNotes)
            {
                Console.WriteLine($"🟢 Received BasmaNote: BasmaId={basma.BasmaId}, Notes={basma.Notes}");
            }
            if (basmaNotes == null || !basmaNotes.Any())
            {
                return Json(new { success = false, message = "No basma notes provided." });
            }

            foreach (var note in basmaNotes)
            {
                var basmaEntry = _context.HREmployeeBasmas.FirstOrDefault(b => b.Id == note.BasmaId);
                if (basmaEntry != null)
                {
                    basmaEntry.Notes = note.Notes;
                    Console.WriteLine($"🟢 Saving note for BasmaId {note.BasmaId}: {note.Notes}");
                }
            }

            _context.SaveChanges();
            return Json(new { success = true });
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpGet]
        public IActionResult SearchEmployees(string query)
        {
            query = query?.Trim() ?? "";

            var list = _context.HREmployeeBasmas.ToList();
            var employees = _context.HREmployees.ToList();

            List<BasmaList> basmaList = new List<BasmaList>();

            foreach (var basma in list)
            {
                var emp = employees.FirstOrDefault(e => e.Id == basma.EmployeeId);

                if (emp != null && emp.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
                {
                    basmaList.Add(new BasmaList
                    {
                        Id = basma.Id,
                        EmployeeName = emp.Name,
                        DayDate = basma.DayDate,
                        ArrivalTime = basma.ArrivalTime,
                        DepartureTime = basma.DepartureTime,
                        TotalHours = basma.TotalHours,
                        LateMinutes = basma.LateMinutes,
                        EarlyLeaveMinutes = basma.EarlyLeaveMinutes,
                        OvertimeMinutes = basma.OvertimeMinutes,
                        OffDayType = basma.OffDayType,
                        Status = basma.Status,
                        Notes = basma.Notes
                    });
                }
            }
            basmaList = basmaList.OrderBy(b => b.EmployeeName).ToList();
            return Json(basmaList);
        }

        //    [HttpGet]
        //    [Route("/getBasmaUsers")]
        //    public IActionResult BasmaUsers()
        //    {
        //        Console.WriteLine("\n========================================");
        //        Console.WriteLine("Testing ZKTeco Fingerprint Device...");
        //        Console.WriteLine("========================================\n");

        //        ZKTecoConnection zkConnection = new ZKTecoConnection();
        //        string deviceIP = "192.168.1.21"; // Change to YOUR device IP
        //        int port = 4370;
        //        if (zkConnection.ConnectToDevice(deviceIP, port))
        //        {
        //            Console.WriteLine("? Connection successful!\n");

        //            //// Get device information
        //            //zkConnection.GetDeviceInfo();

        //            //// Test beep
        //            //Console.WriteLine("\nTesting device beep...");

        //            var users = zkConnection.GetAllUsers();
        //            //if (users[1].)
        //            //// Get device status
        //            //zkConnection.GetDeviceStatus();

        //            // Get all users
        //            zkConnection.DisconnectDevice();
        //            return Json(users);
        //            //// Get attendance logs
        //            //zkConnection.GetAttendanceLogs();

        //            // Disconnect

        //        }
        //        else
        //        {
        //            Console.WriteLine("? Failed to connect to fingerprint device!");
        //            Console.WriteLine("Please check:");
        //            Console.WriteLine("  1. Device IP address is correct: " + deviceIP);
        //            Console.WriteLine("  2. Device is powered on and connected to network");
        //            Console.WriteLine("  3. Port 4370 is not blocked by firewall");
        //            Console.WriteLine("  4. Platform target is set to x86");
        //        }

        //        Console.WriteLine("\n========================================");
        //        Console.WriteLine("Fingerprint test completed.");
        //        Console.WriteLine("========================================\n");
        //        return Json(new {success=false});

        //    }
        //    [HttpGet]
        //    [Route("/basmaDailyEntries")]
        //    public IActionResult BasmaDailyEntries()
        //    {
        //        ZKTecoConnection zkConnection = new ZKTecoConnection();
        //        string deviceIP = "192.168.1.21"; // Change to YOUR device IP
        //        int port = 4370;
        //        if (zkConnection.ConnectToDevice(deviceIP, port))
        //        {
        //            Console.WriteLine("? Connection successful!\n");

        //            //// Get device information
        //            //zkConnection.GetDeviceInfo();

        //            //// Test beep
        //            //Console.WriteLine("\nTesting device beep...");

        //            var basmaEntries = zkConnection.GetAttendanceLogs();
        //            //if (users[1].)
        //            //// Get device status
        //            //zkConnection.GetDeviceStatus();

        //            // Get all users
        //            zkConnection.DisconnectDevice();
        //            return Json(basmaEntries);
        //            //// Get attendance logs
        //            //zkConnection.GetAttendanceLogs();

        //            // Disconnect

        //        }
        //        return Json(new { success = false });
        //    }

    }
}