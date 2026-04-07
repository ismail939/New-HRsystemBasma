using System.Diagnostics;
using HRsystem.Data;
using HRsystem.Models;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using HRsystem.Reports;
using Microsoft.AspNetCore.Authorization;
using HRsystem.ViewModels;
namespace HRsystem.Controllers
{
    public class ReportController : Controller
    {
        private readonly ILogger<ReportController> _logger;
        private readonly AppDbContext _context;
        public ReportController(ILogger<ReportController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpGet]
        [Route("/reports")]
        public IActionResult ReportsPanel()
        {
            return View("ReportsPanel");
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpGet]
        [Route("/PDFReports")]
        public IActionResult PDFReports()
        {
            var departments = _context.HRDepartments.ToList();
            return View("PDFReports", departments);
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpGet]
        [Route("/allInfoReport")]
        public IActionResult AllInfoReport()
        {
            return View("AllInfoReport");
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpGet]
        [Route("/employeesList")]
        public IActionResult EmployeesList()
        {
            var list = _context.HREmployees.ToList();
            return Json(list);
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpGet]
        [Route("/getData")]
        public IActionResult GetData(int employeeId, DateTime startDate, DateTime endDate)
        {
            int arrivalsNumber = _context.HREmployeeBasmas.Where(e => e.EmployeeId == employeeId && e.Status == 0 && e.DayDate >= startDate && e.DayDate <= endDate).Count();
            int absencesNumber = _context.HREmployeeBasmas.Where(e => e.EmployeeId == employeeId && e.Status == 1 && e.DayDate >= startDate && e.DayDate <= endDate).Count();
            int offdays = _context.HREmployeeOffDays.Where(o => o.EmployeeId == employeeId && o.OffDayType != "راحة" && o.OffDayDate >= startDate && o.OffDayDate <= endDate).Count();
            int offs = _context.HREmployeeOffDays.Where(o => o.EmployeeId == employeeId && o.OffDayType == "راحة" && o.OffDayDate >= startDate && o.OffDayDate <= endDate).Count();
            int penalties = _context.HREmployeePenalties.Where(p => p.EmployeeId == employeeId && p.PenaltyDate >= startDate && p.PenaltyDate <= endDate).Count();
            return Json(new { arrivalsNumber = arrivalsNumber, absencesNumber = absencesNumber, offdays = offdays, offs = offs, penalties = penalties });
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpGet]
        [Route("/allInfoReport/getOffDays")]
        public IActionResult GetOffDays(int employeeId, DateTime startDate, DateTime endDate)
        {
            Console.WriteLine(string.Format("🔴employeeId: {0}, startDate: {1}, endDate: {2}", employeeId, startDate, endDate));
            // Example: Query the OffDays table (edit to match your DB)
            var offDays = _context.HREmployeeOffDays
                .Where(o => o.EmployeeId == employeeId &&
                            o.OffDayType != "راحة" &&
                            o.OffDayDate >= startDate &&
                            o.OffDayDate <= endDate)
                .Select(o => new
                {
                    o.OffDayDate,
                    o.OffDayType
                })
                .ToList();

            return Json(offDays);
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpGet]
        [Route("/allInfoReport/getOffs")]
        public IActionResult GetOffs(int employeeId, DateTime startDate, DateTime endDate)
        {
            Console.WriteLine(string.Format("🔴employeeId: {0}, startDate: {1}, endDate: {2}", employeeId, startDate, endDate));
            // Example: Query the OffDays table (edit to match your DB)
            var offDays = _context.HREmployeeOffDays
                .Where(o => o.EmployeeId == employeeId &&
                            o.OffDayType == "راحة" &&
                            o.OffDayDate >= startDate &&
                            o.OffDayDate <= endDate)
                .Select(o => new
                {
                    o.OffDayDate,
                    o.OffDayType
                })
                .ToList();

            return Json(offDays);
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpGet]
        [Route("/allInfoReport/getAbsences")]
        public IActionResult GetAbsences(int employeeId, DateTime startDate, DateTime endDate)
        {
            Console.WriteLine(string.Format("🔴employeeId: {0}, startDate: {1}, endDate: {2}", employeeId, startDate, endDate));
            // Example: Query the OffDays table (edit to match your DB)
            var absences = _context.HREmployeeBasmas
                .Where(o => o.EmployeeId == employeeId &&
                            o.Status == 0 &&
                            o.DayDate >= startDate &&
                            o.DayDate <= endDate)
                .Select(o => new
                {
                    o.DayDate,
                    o.Status
                })
                .ToList();

            return Json(absences);
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpGet]
        [Route("/allInfoReport/getArrivals")]
        public IActionResult GetArrivals(int employeeId, DateTime startDate, DateTime endDate)
        {
            Console.WriteLine(string.Format("🔴employeeId: {0}, startDate: {1}, endDate: {2}", employeeId, startDate, endDate));
            // Example: Query the OffDays table (edit to match your DB)
            var arrivals = _context.HREmployeeBasmas
                .Where(o => o.EmployeeId == employeeId &&
                            o.Status == 0 &&
                            o.DayDate >= startDate &&
                            o.DayDate <= endDate)
                .Select(o => new
                {
                    o.DayDate,
                    o.Status
                })
                .ToList();

            return Json(arrivals);
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpGet]
        [Route("/getRates")]
        public IActionResult GetRates(int employeeId, int startMonth, int endMonth, int startYear, int endYear)
        {
            List<string> months = new List<string>
            {
                "يناير",  // January
                "فبراير", // February
                "مارس",   // March
                "أبريل",  // April
                "مايو",   // May
                "يونيو",  // June
                "يوليو",  // July
                "أغسطس",  // August
                "سبتمبر", // September
                "أكتوبر", // October
                "نوفمبر", // November
                "ديسمبر"  // December
            };

            object empRates;
            List<decimal> rates;
            if (startYear == endYear)
            {
                empRates = _context.HREmployeeRates.Where(r => r.EmployeeId == employeeId && r.Month >= startMonth && r.Month <= endMonth && r.Year == startYear).OrderBy(r => r.Year).Select(e => new MonthRate { MonthName = months[e.Month - 1] + " " + e.Year, Rate = e.Rate }).ToList();
                rates = _context.HREmployeeRates.Where(r => r.EmployeeId == employeeId && r.Month >= startMonth && r.Month <= endMonth && r.Year == startYear).Select(r => r.Rate).ToList();
            }
            else
            {
                empRates = _context.HREmployeeRates.Where(r => r.EmployeeId == employeeId && (r.Month >= startMonth && r.Year == startYear || r.Month <= endMonth && r.Year == endYear)).OrderBy(r => r.Year).Select(e => new MonthRate { MonthName = months[e.Month - 1] + " " + e.Year, Rate = e.Rate }).ToList();
                rates = _context.HREmployeeRates.Where(r => r.EmployeeId == employeeId && (r.Month >= startMonth && r.Year == startYear || r.Month <= endMonth && r.Year == endYear)).Select(r => r.Rate).ToList();
            }

            if (rates.Count == 0)
            {
                return Json(new { success = false });
            }

            var avgRate = rates.Average();
            var o = new { success = true, rates = empRates, avgRate = avgRate };
            return Json(o);
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpGet]
        [Route("/allInfoReport/getPenalties")]
        public IActionResult GetPenalties(int employeeId, DateTime startDate, DateTime endDate)
        {
            Console.WriteLine(string.Format("🔴employeeId: {0}, startDate: {1}, endDate: {2}", employeeId, startDate, endDate));
            // Example: Query the OffDays table (edit to match your DB)
            var penalties = _context.HREmployeePenalties
                .Where(o => o.EmployeeId == employeeId &&
                            o.IsActive == true &&
                            o.PenaltyDate >= startDate &&
                            o.PenaltyDate <= endDate)
                .Select(o => new
                {
                    o.PenaltyDate,
                    o.Decision,
                    o.PenaltyPoints,
                    o.Reason
                })
                .ToList();

            return Json(penalties);
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpGet]
        [Route("/reports/employees")]
        public IActionResult GenerateReport()
        {
            var Employees = _context.HREmployees
                .ToList();
            var employeeVMs = Employees.Select(e => new EmployeeViewModel
            {
                Id = e.Id,
                Name = e.Name,
                NationalId = e.NationalId,
                PhoneNumber = e.PhoneNumber,
                Address = e.Address,
                MarriageStatus = e.MarriageStatus,
                Religion = e.Religion,
                DateOfBirth = e.DateOfBirth,
                InsuranceNumber = e.InsuranceNumber,
                HireDate = e.HireDate,
                EndDate = e.EndDate,
                JobName = e.JobName,
                ContractType = e.ContractType,
                LeaveReason = e.LeaveReason,
                BasmaId = e.BasmaId,
                HRDepartmentId = e.HRDepartmentId,
                Department = _context.HRDepartments.FirstOrDefault(d => d.Id == e.HRDepartmentId)?.Name
            }).ToList();
            var report = new EmployeeReport(employeeVMs);
            var pdf = report.GeneratePdf();

            return File(pdf, "application/pdf", "EmployeesReport.pdf");
        }
        [HttpGet]
        [Route("/test")]
        public IActionResult Test()
        {
            return View("Test");
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpGet]
        [Route("/reports/employeesDH")]
        public IActionResult PrintDailyReport(int id, DateTime startDate, DateTime endDate)
        {
            // 1) Fetch your data
            var emps = _context.HREmployees.Where(e => e.HRDepartmentId == id).ToList();
            if (emps.Count() == 0)
            {
                Console.WriteLine("🧑No employees with hrdepartmentid = 1");
                return NotFound();
            }
            // 2) Build ViewModel
            List<HREmployeeDHVM> list = new List<HREmployeeDHVM>();
            foreach (var emp in emps)
            {
                var BasmaList = _context.HREmployeeBasmas.Where(b => b.EmployeeId == emp.Id && b.DayDate >= startDate && b.DayDate <= endDate).ToList();
                var shifts = _context.HREmployeeShift.ToList();
                var departmentName = _context.HRDepartments.FirstOrDefault(d => d.Id == emp.HRDepartmentId)?.Name ?? "غير محدد";
                var vm = new HREmployeeDHVM
                {
                    DepartmentName = departmentName,
                    TotalRate = _context.HREmployeeRates
                .Where(r => r.EmployeeId == emp.Id)
                .AsEnumerable() // 👈 الحل هنا
                .Where(r =>
                {
                    var monthStart = new DateTime(r.Year, r.Month, 1);
                    var monthEnd = monthStart.AddMonths(1).AddDays(-1);

                    return startDate <= monthStart && endDate >= monthEnd;
                })
                .Select(r => r.Rate)
                .DefaultIfEmpty(0)
                .Average(),
                    BasmaList = BasmaList.Where(b => b.Ok).Select(b => new BasmaWithShiftVM
                    {
                        DayDate = b.DayDate,
                        Arrival = b.ArrivalTime,
                        Departure = b.DepartureTime,
                        Notes = b.Notes,
                        LateMinutes = b.LateMinutes,
                        EarlyLeaveMinutes = b.EarlyLeaveMinutes,
                        OvertimeMinutes = b.OvertimeMinutes,
                        TotalHours = b.TotalHours,
                        Status = b.Status,

                        ShiftStart = shifts
                            .Where(s => b.DayDate.Date >= s.FromDate.Date &&
                                (s.ToDate == null || b.DayDate.Date <= s.ToDate?.Date)
                                && s.EmployeeId == emp.Id)
                            .OrderByDescending(s => s.FromDate)
                            .Select(s => s.StartTime)
                            .FirstOrDefault(),

                        ShiftEnd = shifts
                            .Where(s => b.DayDate.Date >= s.FromDate.Date &&
                                (s.ToDate == null || b.DayDate.Date <= s.ToDate?.Date)
                                && s.EmployeeId == emp.Id)
                            .OrderByDescending(s => s.FromDate)
                            .Select(s => s.EndTime)
                            .FirstOrDefault()
                    }).OrderBy(b => b.DayDate).ToList(),
                    ReportStartDate = startDate,
                    ReportEndDate = endDate,
                    EmployeeName = emp.Name,
                    TotalWorkHours = (float)_context.HREmployeeBasmas
                .Where(b => b.EmployeeId == emp.Id
                            && b.Ok == true
                        && b.DayDate >= startDate
                        && b.DayDate <= endDate)
                    .Sum(b => b.TotalHours ?? 0),
                    TotalLateMinutes = (int)_context.HREmployeeBasmas
                .Where(b => b.EmployeeId == emp.Id
                            && b.Ok == true
                        && b.DayDate >= startDate
                        && b.DayDate <= endDate)
                    .Sum(b => b.LateMinutes ?? 0),
                    TotalEarlyLeaveMinutes = (int)_context.HREmployeeBasmas
                .Where(b => b.EmployeeId == emp.Id
                            && b.Ok == true
                        && b.DayDate >= startDate
                        && b.DayDate <= endDate)
                    .Sum(b => b.EarlyLeaveMinutes ?? 0),
                    TotalOvertimeMinutes = (int)_context.HREmployeeBasmas
                .Where(b => b.EmployeeId == emp.Id
                            && b.Ok == true
                        && b.DayDate >= startDate
                        && b.DayDate <= endDate)
                    .Sum(b => b.OvertimeMinutes ?? 0),

                    EntryDaysCount = _context.HREmployeeBasmas
                .Where(b => b.EmployeeId == emp.Id
                            && b.Status == 1
                            && b.Ok == true
                        && b.DayDate >= startDate
                        && b.DayDate <= endDate)
                .Count(),

                    Leaves = _context.HREmployeeOffDays
                    .Where(a => a.EmployeeId == emp.Id && a.OffDayType != "راحة" && a.OffDayDate >= startDate && a.OffDayDate <= endDate)
                    .Select(a => new LeaveDetail
                    {
                        Date = a.OffDayDate,
                        Type = a.OffDayType
                    })
                    .ToList(),
                    Offs = _context.HREmployeeOffDays
                    .Where(a => a.EmployeeId == emp.Id && a.OffDayType == "راحة" && a.OffDayDate >= startDate && a.OffDayDate <= endDate)
                    .Select(a => new LeaveDetail
                    {
                        Date = a.OffDayDate,
                        Type = a.OffDayType
                    })
                    .ToList(),
                    Ills = _context.HREmployeeOffDays
                    .Where(a => a.EmployeeId == emp.Id && a.OffDayType == "مرضي" && a.OffDayDate >= startDate && a.OffDayDate <= endDate)
                    .Select(a => new LeaveDetail
                    {
                        Date = a.OffDayDate,
                        Type = a.OffDayType
                    })
                    .ToList(),
                    Penalty = _context.HREmployeePenalties
                    .Where(p => p.IsActive && p.EmployeeId == emp.Id && p.PenaltyDate >= startDate && p.PenaltyDate <= endDate)
                    .Select(p => new PenaltyDetail
                    {
                        Date = p.PenaltyDate,
                        Decision = p.Decision,
                        Reason = p.Reason
                    })
                    .ToList(),
                    Absences = _context.HREmployeeBasmas
                    .Where(a => a.EmployeeId == emp.Id && a.DayDate >= startDate && a.DayDate <= endDate && a.Ok == true && a.Status == 0)
                    .Select(a => new AbsenceDetail
                    {
                        Date = a.DayDate
                    })
                    .ToList()

                };
                list.Add(vm);
            }

            // 3) Create the report
            var report = new EmployeeReportDHVM(list);

            // 4) Generate PDF 
            var pdfBytes = report.GeneratePdf();

            // 5) Return PDF
            return File(pdfBytes, "application/pdf", $"EmployeeReport_.pdf");
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpGet]
        [Route("/reports/employeesDaily")]
        public IActionResult PrintEmpDailyReport(int id, DateTime startDate, DateTime endDate)
        {
            // 🔹 1) Get employees in department
            var emps = _context.HREmployees
                .Where(e => e.HRDepartmentId == id)
                .ToList();

            if (!emps.Any())
                return NotFound("No employees found");

            // 🔹 2) Get all shifts once
            var allShifts = _context.HREmployeeShift.ToList();

            // 🔹 3) Department name
            var departmentName = _context.HRDepartments
                .FirstOrDefault(d => d.Id == id)?.Name ?? "غير محدد";

            // 🔹 4) Final list
            List<HREmployeeDHVM> list = new List<HREmployeeDHVM>();

            foreach (var emp in emps)
            {
                // ✅ basmas for employee
                var basmas = _context.HREmployeeBasmas
                    .Where(b => b.EmployeeId == emp.Id
                        && b.Ok == true
                        && b.DayDate >= startDate
                        && b.DayDate <= endDate)
                    .ToList();

                // ✅ shifts for employee
                var empShifts = allShifts
                    .Where(s => s.EmployeeId == emp.Id)
                    .OrderByDescending(s => s.FromDate)
                    .ToList();

                // ✅ helper
                HREmployeeShift? GetShift(DateTime date)
                {
                    return empShifts.FirstOrDefault(s =>
                        date.Date >= s.FromDate.Date &&
                        (s.ToDate == null || date.Date <= s.ToDate.Value.Date)
                    );
                }

                // ✅ build VM
                var vm = new HREmployeeDHVM
                {
                    DepartmentName = departmentName,
                    EmployeeName = emp.Name,
                    ReportStartDate = startDate,
                    ReportEndDate = endDate,

                    // 🔥 basma + shift
                    BasmaList = basmas
                        .Select(b =>
                        {
                            var shift = GetShift(b.DayDate);

                            return new BasmaWithShiftVM
                            {
                                DayDate = b.DayDate,
                                Arrival = b.ArrivalTime,
                                Departure = b.DepartureTime,
                                Notes = b.Notes,
                                LateMinutes = b.LateMinutes,
                                EarlyLeaveMinutes = b.EarlyLeaveMinutes,
                                OvertimeMinutes = b.OvertimeMinutes,
                                TotalHours = b.TotalHours,
                                Status = b.Status,

                                ShiftStart = shift?.StartTime,
                                ShiftEnd = shift?.EndTime
                            };
                        })
                        .OrderBy(b => b.DayDate)
                        .ToList(),

                    // 🔥 totals
                    TotalWorkHours = (float)basmas.Sum(b => b.TotalHours ?? 0),
                    TotalLateMinutes = basmas.Sum(b => b.LateMinutes ?? 0),
                    TotalEarlyLeaveMinutes = basmas.Sum(b => b.EarlyLeaveMinutes ?? 0),
                    TotalOvertimeMinutes = basmas.Sum(b => b.OvertimeMinutes ?? 0),
                    EntryDaysCount = basmas.Count(b => b.Status == 1),

                    // 🔹 absences
                    Absences = basmas
                        .Where(a => a.Status == 0)
                        .Select(a => new AbsenceDetail
                        {
                            Date = a.DayDate
                        })
                        .ToList()
                };

                list.Add(vm);
            }

            // 🔹 5) generate report for ALL employees
            var report = new DailyReport();

            var pdfBytes = report.GeneratePdf();

            return File(pdfBytes, "application/pdf", $"EmployeesReport.pdf");
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpGet]
        [Route("/reports/managersDH")]
        public IActionResult PrintManagersReport(DateTime startDate, DateTime endDate)
        {
            // 1) Fetch your data
            var managerIds = _context.HRDepartments.Where(e => e.ManagerId != null).Select(p => p.ManagerId).ToList();
            var emps = _context.HREmployees.Where(e => managerIds.Contains(e.Id)).ToList();
            if (emps.Count() == 0)
            {
                return NotFound();
            }
            // 2) Build ViewModel
            List<HREmployeeDHVM> list = new List<HREmployeeDHVM>();
            foreach (var emp in emps)
            {
                var BasmaList = _context.HREmployeeBasmas.Where(b => b.EmployeeId == emp.Id && b.DayDate >= startDate && b.DayDate <= endDate).ToList();
                var shifts = _context.HREmployeeShift.ToList();
                var departmentName = _context.HRDepartments.FirstOrDefault(d => d.Id == emp.HRDepartmentId)?.Name ?? "غير محدد";
                var vm = new HREmployeeDHVM
                {
                    DepartmentName = departmentName,
                    TotalRate = _context.HREmployeeRates
                .Where(r => r.EmployeeId == emp.Id)
                .AsEnumerable() // 👈 الحل هنا
                .Where(r =>
                {
                    var monthStart = new DateTime(r.Year, r.Month, 1);
                    var monthEnd = monthStart.AddMonths(1).AddDays(-1);

                    return startDate <= monthStart && endDate >= monthEnd;
                })
                .Select(r => r.Rate)
                .DefaultIfEmpty(0)
                .Average(),
                    BasmaList = BasmaList.Where(b => b.Ok).Select(b => new BasmaWithShiftVM
                    {
                        DayDate = b.DayDate,
                        Arrival = b.ArrivalTime,
                        Departure = b.DepartureTime,
                        Notes = b.Notes,
                        LateMinutes = b.LateMinutes,
                        EarlyLeaveMinutes = b.EarlyLeaveMinutes,
                        OvertimeMinutes = b.OvertimeMinutes,
                        TotalHours = b.TotalHours,
                        Status = b.Status,

                        ShiftStart = shifts
                            .Where(s => b.DayDate.Date >= s.FromDate.Date &&
                                (s.ToDate == null || b.DayDate.Date <= s.ToDate?.Date)
                                && s.EmployeeId == emp.Id)
                            .OrderByDescending(s => s.FromDate)
                            .Select(s => s.StartTime)
                            .FirstOrDefault(),

                        ShiftEnd = shifts
                            .Where(s => b.DayDate.Date >= s.FromDate.Date &&
                                (s.ToDate == null || b.DayDate.Date <= s.ToDate?.Date)
                                && s.EmployeeId == emp.Id)
                            .OrderByDescending(s => s.FromDate)
                            .Select(s => s.EndTime)
                            .FirstOrDefault()
                    }).OrderBy(b => b.DayDate).ToList(),
                    ReportStartDate = startDate,
                    ReportEndDate = endDate,
                    EmployeeName = emp.Name,
                    TotalWorkHours = (float)_context.HREmployeeBasmas
                .Where(b => b.EmployeeId == emp.Id
                            && b.Ok == true
                        && b.DayDate >= startDate
                        && b.DayDate <= endDate)
                    .Sum(b => b.TotalHours ?? 0),
                    TotalLateMinutes = (int)_context.HREmployeeBasmas
                .Where(b => b.EmployeeId == emp.Id
                            && b.Ok == true
                        && b.DayDate >= startDate
                        && b.DayDate <= endDate)
                    .Sum(b => b.LateMinutes ?? 0),
                    TotalEarlyLeaveMinutes = (int)_context.HREmployeeBasmas
                .Where(b => b.EmployeeId == emp.Id
                            && b.Ok == true
                        && b.DayDate >= startDate
                        && b.DayDate <= endDate)
                    .Sum(b => b.EarlyLeaveMinutes ?? 0),
                    TotalOvertimeMinutes = (int)_context.HREmployeeBasmas
                .Where(b => b.EmployeeId == emp.Id
                            && b.Ok == true
                        && b.DayDate >= startDate
                        && b.DayDate <= endDate)
                    .Sum(b => b.OvertimeMinutes ?? 0),
                    EntryDaysCount = _context.HREmployeeBasmas
                .Where(b => b.EmployeeId == emp.Id
                            && b.Status == 1
                            && b.Ok == true
                        && b.DayDate >= startDate
                        && b.DayDate <= endDate)
                .Count(),

                    Leaves = _context.HREmployeeOffDays
                    .Where(a => a.EmployeeId == emp.Id && a.OffDayType != "راحة" && a.OffDayDate >= startDate && a.OffDayDate <= endDate)
                    .Select(a => new LeaveDetail
                    {
                        Date = a.OffDayDate,
                        Type = a.OffDayType
                    })
                    .ToList(),
                    Offs = _context.HREmployeeOffDays
                    .Where(a => a.EmployeeId == emp.Id && a.OffDayType == "راحة" && a.OffDayDate >= startDate && a.OffDayDate <= endDate)
                    .Select(a => new LeaveDetail
                    {
                        Date = a.OffDayDate,
                        Type = a.OffDayType
                    })
                    .ToList(),
                    Ills = _context.HREmployeeOffDays
                    .Where(a => a.EmployeeId == emp.Id && a.OffDayType == "مرضي" && a.OffDayDate >= startDate && a.OffDayDate <= endDate)
                    .Select(a => new LeaveDetail
                    {
                        Date = a.OffDayDate,
                        Type = a.OffDayType
                    })
                    .ToList(),
                    Penalty = _context.HREmployeePenalties
                    .Where(p => p.IsActive && p.EmployeeId == emp.Id && p.PenaltyDate >= startDate && p.PenaltyDate <= endDate)
                    .Select(p => new PenaltyDetail
                    {
                        Date = p.PenaltyDate,
                        Decision = p.Decision,
                        Reason = p.Reason
                    })
                    .ToList(),
                    Absences = _context.HREmployeeBasmas
                    .Where(a => a.EmployeeId == emp.Id && a.DayDate >= startDate && a.DayDate <= endDate && a.Ok == true && a.Status == 0)
                    .Select(a => new AbsenceDetail
                    {
                        Date = a.DayDate
                    })
                    .ToList()

                };
                list.Add(vm);
            }

            // 3) Create the report
            var report = new ManagersReport(list);

            // 4) Generate PDF 
            var pdfBytes = report.GeneratePdf();

            // 5) Return PDF
            return File(pdfBytes, "application/pdf", $"ManagersReport_{startDate}_to_{endDate}.pdf");
        }


    }
}