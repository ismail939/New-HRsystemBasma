 using System.Diagnostics;
using System.Security.Claims;
using HRsystem.Data;
using HRsystem.Models;
using HRsystem.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRsystem.Controllers
{
    public class EmployeeDashboardController : Controller
    {
        private readonly ILogger<EmployeeDashboardController> _logger;
        private readonly AppDbContext _context;

        public EmployeeDashboardController(ILogger<EmployeeDashboardController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Authorize(Roles = "Employee")]
        [HttpGet]
        [Route("/employee-dashboard")]
        public IActionResult Dashboard()
        {
            string nationalId = User.FindFirst("NationalId")?.Value ?? "";
            var employee = _context.HREmployees.FirstOrDefault(e => e.NationalId == nationalId);
            if (employee == null)
            {
                return View(new EmployeeDashboardViewModel
                {
                    EmployeeName = "غير معروف",
                    Days = GenerateDays(new List<HREmployeeOffDay>())
                });
            }

            var offDayBalances = _context.HROffDayBalances.FirstOrDefault(b => b.EmployeeId == employee.Id);
            var penalties = _context.HREmployeePenalties.Where(p => p.EmployeeId == employee.Id && p.IsActive).ToList();
            var offDays = _context.HREmployeeOffDays.Where(o => o.EmployeeId == employee.Id).ToList();

            // Determine next month name in Arabic
            var nextMonth = DateTime.Today.AddMonths(1);
            string nextMonthName = GetArabicMonthName(nextMonth.Month);

            // Generate sensible salary data based on job role
            var basicSalary = employee.JobName switch
            {
                "مدير" => 15000m,
                "محاسب" => 8000m,
                "مبرمج" => 12000m,
                "مهندس" => 10000m,
                "موظف إداري" => 6000m,
                "سكرتير" => 5000m,
                _ => 7000m
            };
            decimal deductions = basicSalary * 0.085m; // 8.5% deductions (insurance, tax, etc.)
            decimal netSalary = basicSalary - deductions;

            var vm = new EmployeeDashboardViewModel
            {
                EmployeeName = employee.Name,
                AnnualBalance = offDayBalances?.Annual ?? 0,
                CasualBalance = offDayBalances?.Casual ?? 0,
                OffBalance = offDayBalances?.Off ?? 0,
                CompensatoryBalance = offDayBalances?.CompensatoryOfNationalHoliday ?? 0,
                ActivePenaltiesCount = penalties.Count,
                TotalPenaltyPoints = penalties.Sum(p => p.PenaltyPoints),
                BasicSalary = basicSalary,
                NetSalary = netSalary,
                YearToDateEarnings = netSalary * DateTime.Today.Month, // accumulated from Jan till now
                NextPaymentDate = $"{nextMonth.Day} {nextMonthName} {nextMonth.Year}",
                UpcomingMonthSalary = netSalary,
                UpcomingMonthName = nextMonthName,
                Days = GenerateDays(offDays)
            };

            return View(vm);
        }

        private string GetArabicMonthName(int month)
        {
            string[] arabicMonths = { "يناير", "فبراير", "مارس", "أبريل", "مايو", "يونيو", "يوليو", "أغسطس", "سبتمبر", "أكتوبر", "نوفمبر", "ديسمبر" };
            return arabicMonths[month - 1];
        }

        private List<DayStatus> GenerateDays(List<HREmployeeOffDay> offDays)
        {
            var days = new List<DayStatus>();
            var today = DateTime.Today;

            // Show 60 days in the past, 60 days in the future = 120 total + today
            for (int i = -60; i <= 60; i++)
            {
                var date = today.AddDays(i);
                var offDay = offDays.FirstOrDefault(o => o.OffDayDate.Date == date.Date);

                days.Add(new DayStatus
                {
                    Date = date,
                    DayName = date.ToString("dddd", new System.Globalization.CultureInfo("ar-SA")),
                    DayNumber = date.Day.ToString(),
                    MonthName = GetArabicMonthName(date.Month),
                    IsOffDay = offDay != null,
                    OffDayType = offDay?.OffDayType,
                    IsPast = date < today,
                    IsToday = date == today
                });
            }

            return days;
        }
        [Authorize(Roles = "Employee")]
        [HttpGet]
        [Route("/employee-dashboard/change-password")]
        public IActionResult ChangePassword()
        {
            string username = User.FindFirst("NationalId")?.Value;
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return NotFound();
            }
           
            return View();
        }
        [Authorize(Roles = "Employee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/employee-dashboard/change-password")]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string username = User.FindFirst("NationalId")?.Value;
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return NotFound();
            }
            if(model.ConfirmNewPassword != model.NewPassword)
            {
                ModelState.AddModelError("", "كلمة المرور الجديدة وتأكيدها غير متطابقين");
                return View(model);
            }
            user.Password = PasswordHasher.HashPassword(model.NewPassword);
            user.IsActive = true;
            _context.SaveChanges();

            return RedirectToAction("Dashboard");
        }

    }
}
