using System.Diagnostics;
using System.Security.Claims;
using HRsystem.Data;
using HRsystem.Helpers;
using HRsystem.Models;
using HRsystem.Services;
using HRsystem.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRsystem.Controllers
{
    public class EmployeeDashboardController : Controller
    {
        private readonly ILogger<EmployeeDashboardController> _logger;
        private readonly AppDbContext _context;
        private readonly IOffDayBalanceAutoService _balanceService;

        public EmployeeDashboardController(
            ILogger<EmployeeDashboardController> logger, 
            AppDbContext context,
            IOffDayBalanceAutoService balanceService)
        {
            _logger = logger;
            _context = context;
            _balanceService = balanceService;
        }

        [Authorize(Roles = "Employee")]
        [HttpGet]
        [Route("/employee-dashboard")]
        public async Task<IActionResult> Dashboard()
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

            // Get or auto-calculate balance
            var offDayBalances = await _balanceService.GetOrCalculateBalanceAsync(employee.Id);
            var penalties = _context.EmployeePenalties.Where(p => p.EmployeeId == employee.Id && p.Status == HRsystem.Models.Enums.PenaltyStatus.Approved).ToList();
            var offDays = _context.HREmployeeOffDays.Where(o => o.EmployeeId == employee.Id).ToList();

            // Determine next month name in Arabic
            var nextMonth = DateTime.Today.AddMonths(1);
            string nextMonthName = GetArabicMonthName(nextMonth.Month);

            var salaryComponents = _context.EmployeePayrollComponents
                .Include(x => x.PayrollComponent)
                .Where(x => x.EmployeeId == employee.Id && x.IsActive)
                .ToList();
            var basicSalary = salaryComponents.FirstOrDefault(x => x.PayrollComponent.Category == Models.Enums.PayrollComponentCategory.Salary)?.Amount ?? 0m;
            var grossSalary = salaryComponents.Where(x => x.PayrollComponent.Category != Models.Enums.PayrollComponentCategory.Deduction).Sum(x => x.Amount);
            var netSalary = grossSalary;
            var currentPayroll = _context.PayrollDetails.Include(x => x.Payroll)
                .Where(x => x.EmployeeId == employee.Id && (x.Payroll.Status == Models.Enums.PayrollStatus.Approved || x.Payroll.Status == Models.Enums.PayrollStatus.Locked))
                .OrderByDescending(x => x.Payroll.Year).ThenByDescending(x => x.Payroll.Month).FirstOrDefault();
            var payslips = _context.PayrollDetails.Include(x => x.Payroll)
                .Where(x => x.EmployeeId == employee.Id && (x.Payroll.Status == Models.Enums.PayrollStatus.Approved || x.Payroll.Status == Models.Enums.PayrollStatus.Locked))
                .OrderByDescending(x => x.Payroll.Year).ThenByDescending(x => x.Payroll.Month).Take(12).ToList();
            if (currentPayroll != null) { basicSalary = currentPayroll.BasicSalary; netSalary = currentPayroll.NetSalary; grossSalary = currentPayroll.GrossSalary; }
            var history = _context.PayrollComponentHistories.Include(x => x.PayrollComponent)
                .Where(x => x.EmployeeId == employee.Id).OrderByDescending(x => x.EffectiveDate).Take(20).ToList();

            var vm = new EmployeeDashboardViewModel
            {
                EmployeeName = employee.Name,
                AnnualBalance = offDayBalances.Annual,
                CasualBalance = offDayBalances.Casual,
                SickBalance = offDayBalances.Sick,
                HajjBalance = offDayBalances.Hajj,
                MaternityBalance = offDayBalances.Maternity,
                UnpaidBalance = offDayBalances.Unpaid,
                CompensatoryBalance = offDayBalances.Compensatory,
                OfficialHolidayBalance = offDayBalances.OfficialHoliday,
                ExamBalance = offDayBalances.Exam,
                OffBalance = offDayBalances.Unpaid, // backward compat
                ActivePenaltiesCount = penalties.Count,
                TotalPenaltyPoints = penalties.Count,
                BasicSalary = basicSalary,
                NetSalary = netSalary,
                YearToDateEarnings = payslips.Where(x => x.Payroll.Year == DateTime.Today.Year).Sum(x => x.NetSalary),
                NextPaymentDate = $"{nextMonth.Day} {nextMonthName} {nextMonth.Year}",
                UpcomingMonthSalary = netSalary,
                SalaryHistory = history.Select(x => new EmployeeSalaryHistoryItem { EffectiveDate = x.EffectiveDate, ComponentName = x.PayrollComponent.NameAr, OldAmount = x.OldAmount, NewAmount = x.NewAmount, Reason = x.Reason }).ToList(),
                Payslips = payslips.Select(x => new EmployeePayslipItem { PayrollId = x.PayrollId, Period = $"{x.Payroll.Month}/{x.Payroll.Year}", BasicSalary = x.BasicSalary, GrossSalary = x.GrossSalary, TotalDeductions = x.TotalDeductions, TaxAmount = x.TaxAmount, InsuranceAmount = x.InsuranceAmount, NetSalary = x.NetSalary }).ToList(),
                UpcomingMonthName = nextMonthName,
                Days = GenerateDays(offDays)
            };

            return View(vm);
        }

        private string GetArabicMonthName(int month) => MonthNamesHelper.GetGregorianMonthName(month);

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
