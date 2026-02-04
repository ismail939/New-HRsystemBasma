using System.Diagnostics;
using HRsystem.Data;
using HRsystem.Models;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using HRsystem.Reports;
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
        [HttpGet]
        [Route("/reports")]
        public IActionResult ReportsPanel()
        {
            return View("ReportsPanel");
        }
        [HttpGet]
        [Route("/PDFReports")]
        public IActionResult PDFReports()
        {
            return View("PDFReports");
        }
        [HttpGet]
        [Route("/allInfoReport")]
        public IActionResult AllInfoReport()
        {
            return View("AllInfoReport");
        }
        [HttpGet]
        [Route("/employeesList")]
        public IActionResult EmployeesList()
        {
            var list = _context.HREmployees.ToList();
            return Json(list);
        }
        
        [HttpGet]
        [Route("/getData")]
        public IActionResult GetData(int employeeId, DateTime startDate, DateTime endDate)
        {
            int arrivalsNumber = _context.HREmployeeBasmas.Where(e=>e.EmployeeId==employeeId&&e.Status == 0&& e.DayDate>=startDate && e.DayDate<=endDate).Count();
            int absencesNumber = _context.HREmployeeBasmas.Where(e=>e.EmployeeId==employeeId&&e.Status == 1&& e.DayDate>=startDate && e.DayDate<=endDate).Count();
            int offdays = _context.HREmployeeOffDays.Where(o=>o.EmployeeId==employeeId&&o.OffDayType!="راحة"&&o.OffDayDate>=startDate && o.OffDayDate<=endDate).Count();
            int offs = _context.HREmployeeOffDays.Where(o=>o.EmployeeId==employeeId&&o.OffDayType=="راحة"&&o.OffDayDate>=startDate && o.OffDayDate<=endDate).Count();
            int penalties = _context.HREmployeePenalties.Where(p=>p.EmployeeId==employeeId&&p.PenaltyDate>=startDate && p.PenaltyDate<=endDate).Count();
            return Json(new{arrivalsNumber=arrivalsNumber, absencesNumber=absencesNumber, offdays=offdays, offs=offs, penalties=penalties}); 
        }

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
            List <decimal> rates;
            if (startYear == endYear)
            {
                empRates = _context.HREmployeeRates.Where(r=>r.EmployeeId==employeeId&&r.Month>=startMonth&&r.Month<=endMonth&&r.Year==startYear).OrderBy(r=>r.Year).Select(e=>new MonthRate{MonthName=months[e.Month-1]+" "+e.Year,Rate=e.Rate}).ToList();
                rates = _context.HREmployeeRates.Where(r=>r.EmployeeId==employeeId&&r.Month>=startMonth&&r.Month<=endMonth&&r.Year==startYear).Select(r=>r.Rate).ToList();
            }
            else
            {
                empRates = _context.HREmployeeRates.Where(r=>r.EmployeeId==employeeId&&(r.Month>=startMonth&&r.Year==startYear||r.Month<=endMonth&&r.Year==endYear)).OrderBy(r=>r.Year).Select(e=>new MonthRate{MonthName=months[e.Month-1]+" "+e.Year,Rate=e.Rate}).ToList();
                rates = _context.HREmployeeRates.Where(r=>r.EmployeeId==employeeId&&(r.Month>=startMonth&&r.Year==startYear||r.Month<=endMonth&&r.Year==endYear)).Select(r=>r.Rate).ToList();
            }
            
            if (rates.Count == 0)
            {
                return Json(new { success = false });
            }

            var avgRate = rates.Average();
            var o = new { success = true, rates = empRates, avgRate = avgRate };
            return Json(o);
        }

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
        [HttpGet]
        [Route("/reports/employees")]
        public IActionResult GenerateReport()
        {
            var Employees = _context.HREmployees
                .ToList();
            var report = new EmployeeReport(Employees);
            var pdf = report.GeneratePdf();

            return File(pdf, "application/pdf", "EmployeesReport.pdf");
        }
        [HttpGet]
        [Route("/test")]
        public IActionResult Test()
        {
            return View("Test");
        }
        [HttpGet]
        [Route("/reports/employeesDH/{id}")]
        public IActionResult PrintDailyReport(int id)
        {
            // 1) Fetch your data
            var emp = _context.HREmployees.Find(id);
            if (emp == null)
            {
                return NotFound();
            }
            // 2) Build ViewModel
            var vm = new HREmployeeDHVM
            {
                EmployeeName = emp.Name,
                EntryDaysCount = _context.HREmployeeBasmas
                .Where(b => b.EmployeeId == id
                        && b.ArrivalTime != null
                        && b.DepartureTime != null)
                .Count(),

                Leaves = _context.HREmployeeOffDays
                    .Where(a => a.EmployeeId == id)
                    .Select(a => new LeaveDetail
                    {
                        Date = a.OffDayDate,
                        Type = a.OffDayType
                    })
                    .ToList(),
                Penalty = _context.HREmployeePenalties
                    .Where(p => p.EmployeeId == id)
                    .Select(p => new PenaltyDetail
                    {
                        Date = p.PenaltyDate,
                        Decision = p.Decision,
                        Reason = p.Reason
                    })
                    .ToList(),
                Absences = _context.HREmployeeBasmas
                    .Where(a => a.EmployeeId == id)
                    .Select(a => new AbsenceDetail
                    {
                        Date = a.DayDate
                    })
                    .ToList()
            };

            // 3) Create the report
            var report = new EmployeeReportDHVM(vm);

            // 4) Generate PDF 
            var pdfBytes = report.GeneratePdf();

            // 5) Return PDF
            return File(pdfBytes, "application/pdf", $"EmployeeReport_{emp.Name}.pdf");
        }

    }
}