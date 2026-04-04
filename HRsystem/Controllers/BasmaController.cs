using HRsystem.Data;
using HRsystem.Models;
using HRsystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            public int LateMinutes { get; set; }
            public int EarlyLeaveMinutes { get; set; }
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
                        Ok = basma.Ok,
                        Status = basma.Status,
                        Notes = basma.Notes
                    });
                }
            }
            ;
            basmaList = basmaList.OrderBy(b => b.EmployeeName).ToList();
            basmaList.ForEach(p => Console.WriteLine($"🟢 Basma List Entry: {p.EmployeeName}, {p.DayDate}, {p.ArrivalTime}, {p.DepartureTime}, {p.TotalHours}, {p.LateMinutes}, {p.EarlyLeaveMinutes}, {p.Status}, {p.Notes}"));
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
            basmaEntry.Ok = false;
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
                Console.WriteLine("🟢 Entered future date");
                return RedirectToAction("GetDayBasma", "Basma", new { Day = Day.Date });
            }

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                Console.WriteLine("🟢 Start Sync Basma");

                // 🟢 Load existing basmas once
                var existingBasmas = _context.HREmployeeBasmas
                    .Where(b => b.DayDate == Day.Date)
                    .ToList();

                var basmaDict = existingBasmas.ToDictionary(b => b.EmployeeId);

                // 🟢 Load shifts once
                var employeeShifts = _context.HREmployeeShift
                    .Where(emp => emp.ToDate == null || emp.ToDate >= Day.Date)
                    .ToList();

                var start = Day.Date;
                var end = Day.Date.AddDays(1).AddHours(3);

                var checkInOutsGrouped = _context.CheckInOuts
                    .Where(cio => cio.CheckTime >= start && cio.CheckTime <= end)
                    .GroupBy(cio => cio.UserId)
                    .ToList();

                // 🟢 PROCESS employees WITH fingerprints
                foreach (var userGroup in checkInOutsGrouped)
                {
                    var userId = userGroup.Key;
                    var checkTimes = userGroup.ToList();
                    var employee = _context.HREmployees
                        .FirstOrDefault(e => e.BasmaId == userId);

                    if (employee == null)
                        continue;

                    if (basmaDict.ContainsKey(employee.Id) && basmaDict[employee.Id].Ok)
                    {
                        continue; // already processed with fingerprint data
                    }
                    DateTime arrivalTime = checkTimes.Min(ct => ct.CheckTime);
                    DateTime departureTime = checkTimes.Max(ct => ct.CheckTime);

                    LatencyResult latency;
                    if (checkTimes.Count == 1)
                    {
                        latency = new LatencyResult
                        {
                            LateMinutes = 0,
                            EarlyLeaveMinutes = 0,
                            TotalHours = 0
                        };
                    }
                    else
                    {
                        latency =
                            LatencyForEmployee(employeeShifts, employee.Id, arrivalTime, departureTime);

                    }

                    if (!basmaDict.TryGetValue(employee.Id, out var basma))
                    {
                        // 🟢 CREATE
                        basma = new HREmployeeBasma
                        {
                            EmployeeId = employee.Id,
                            DayDate = Day.Date
                        };

                        _context.HREmployeeBasmas.Add(basma);
                        basmaDict[employee.Id] = basma;
                    }

                    // 🟡 UPDATE (always)
                    basma.ArrivalTime = arrivalTime;
                    basma.DepartureTime = departureTime;
                    basma.LateMinutes = latency.LateMinutes;
                    basma.EarlyLeaveMinutes = latency.EarlyLeaveMinutes;
                    basma.TotalHours = latency.TotalHours;
                    basma.Ok = checkTimes.Count >= 1;
                    basma.Status = 1;
                }

                // 🟢 PROCESS employees WITHOUT fingerprints
                var allEmployees = _context.HREmployees.ToList();

                foreach (var emp in allEmployees)
                {
                    if (basmaDict.ContainsKey(emp.Id) && basmaDict[emp.Id].Ok)
                    {
                        continue; // already processed with fingerprint data
                    }
                    if (basmaDict.ContainsKey(emp.Id))
                    {
                        bool isOffDaY = _context.HREmployeeOffDays
                        .Any(od => od.EmployeeId == emp.Id && od.OffDayDate.Date == Day.Date);
                        if (isOffDaY)
                        {
                            var existingBasma = basmaDict[emp.Id];
                            existingBasma.Ok = true;
                            existingBasma.Status = 2; // off day
                            _context.SaveChanges();
                        }
                        continue;
                    }

                    bool isOffDay = _context.HREmployeeOffDays
                        .Any(od => od.EmployeeId == emp.Id && od.OffDayDate.Date == Day.Date);

                    var basma = new HREmployeeBasma
                    {
                        EmployeeId = emp.Id,
                        DayDate = Day.Date,
                        ArrivalTime = null,
                        DepartureTime = null,
                        Ok = isOffDay,
                        Status = isOffDay ? 2 : 3 // 2 = off day, 3 = absent
                    };

                    _context.HREmployeeBasmas.Add(basma);
                    basmaDict[emp.Id] = basma;
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

        [Authorize(Roles = "Admin,HR")]
        private LatencyResult LatencyForEmployee(List<HREmployeeShift> employeeShifts, int employeeId, DateTime checkIn, DateTime checkOut)
        {
            var shift = employeeShifts.FirstOrDefault(es => es.EmployeeId == employeeId);
            if (shift == null)
            {
                return new LatencyResult
                {
                    LateMinutes = 0,
                    EarlyLeaveMinutes = 0,
                    TotalHours = 0
                };
            }

            if (shift.ShiftMode == 0)
            {
                return new LatencyResult
                {
                    LateMinutes = 0,
                    EarlyLeaveMinutes = 0,
                    TotalHours = checkOut.Subtract(checkIn).Hours
                };
            }
            else if (shift.ShiftMode == 1)
            {
                double workedHours = Math.Round((checkOut - checkIn).TotalMinutes / 60.0, 1);

                var requiredTimeSpan = TimeSpan.FromHours(shift.RequiredHours ?? 0);
                double missingHours = Math.Round(
                    Math.Max(0, (requiredTimeSpan - (checkOut - checkIn)).TotalMinutes) / 60.0,
                    1
                );
                Console.WriteLine("🟢🟢🟢🟢🟢🟢🟢🟢🟢🟢");
                Console.WriteLine("🟢" + checkOut.Subtract(checkIn).TotalMinutes / 60.0f);
                return new LatencyResult
                {
                    LateMinutes = 0,
                    EarlyLeaveMinutes = (int)requiredTimeSpan.Subtract(checkOut.Subtract(checkIn)).TotalMinutes,
                    TotalHours = (float)checkOut.Subtract(checkIn).TotalMinutes / 60.0f
                };
            }
            else if (shift.ShiftMode == 2)
            {
                double workedHours = Math.Round((checkOut - checkIn).TotalMinutes / 60.0, 1);

                var requiredTimeSpan = TimeSpan.FromHours(shift.RequiredHours ?? 0);
                double missingHours = Math.Round(
                    Math.Max(0, (requiredTimeSpan - (checkOut - checkIn)).TotalMinutes) / 60.0,
                    1
                );
                var checkInTime = checkIn.TimeOfDay;
                var shiftStartTime = (shift.StartTime ?? checkIn).TimeOfDay;

                var lateMinutes = (int)Math.Max(
                    0,
                    (checkInTime - shiftStartTime).TotalMinutes
                );
                var earlyLeaveMinutes = (int)Math.Max(
                    0,
                    ((shift.EndTime ?? checkOut).TimeOfDay - checkOut.TimeOfDay).TotalMinutes
                );

                Console.WriteLine("🟢🟢🟢🟢🟢🟢");
                Console.WriteLine("🟢" + checkOut.Subtract(checkIn).TotalMinutes / 60.0f);
                return new LatencyResult
                {
                    LateMinutes = lateMinutes,
                    EarlyLeaveMinutes = earlyLeaveMinutes,
                    TotalHours = (float)checkOut.Subtract(checkIn).TotalMinutes / 60.0f
                };
            }
            return new LatencyResult
            {
                LateMinutes = 0,
                EarlyLeaveMinutes = 0,
                TotalHours = 0
            };
        }


        [Authorize(Roles = "Admin,HR")]
        [HttpPost]
        [Route("/confirmBasma")]
        public IActionResult ConfirmBasma(int id, int type)
        {
            var basmaEntry = _context.HREmployeeBasmas.FirstOrDefault(b => b.Id == id);
            if (basmaEntry != null)
            {
                basmaEntry.Ok = true;
                if (type == 2)
                {
                    // ask whether there is an offday
                    var isOffDay = _context.HREmployeeOffDays
                        .Any(od => od.EmployeeId == basmaEntry.EmployeeId && od.OffDayDate.Date == basmaEntry.DayDate.Date);
                    Console.WriteLine($"🟢 Checking off day for EmployeeId {basmaEntry.Id} on {basmaEntry.DayDate.Date}: {isOffDay}");
                    if (isOffDay)
                    {
                        basmaEntry.Status = 2; // offday
                        _context.SaveChanges();
                        return Json(new { code = 1 });
                    }
                    return Json(new { code = 2, message = "No off day found for the employee on that date." });
                }
                else if (type == 0)
                {
                    basmaEntry.Status = 0; // absent
                    _context.SaveChanges();
                    return Json(new { code = 1 });
                }
                else if (type == 1)
                {
                    basmaEntry.Status = 1; // present
                    _context.SaveChanges();
                    return Json(new { code = 1 });
                }
            }
            return Json(new { success = false, message = "Basma entry not found." });
        }

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