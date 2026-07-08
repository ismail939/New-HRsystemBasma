using HRsystem.Data;
using HRsystem.Models;
using HRsystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRsystem.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    public class PayrollController : Controller
    {
        private readonly ILogger<PayrollController> _logger;
        private readonly AppDbContext _context;

        public PayrollController(ILogger<PayrollController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // ============ SALARY COMPONENTS ============

        [HttpGet]
        [Route("/payroll/salary-components")]
        public IActionResult SalaryComponents()
        {
            var components = _context.SalaryComponents
                .OrderBy(c => c.Type)
                .ThenBy(c => c.Name)
                .ToList();
            return View(components);
        }

        [HttpPost]
        [Route("/payroll/salary-components/add")]
        public IActionResult AddSalaryComponent(SalaryComponent component)
        {
            if (string.IsNullOrEmpty(component.Name) || string.IsNullOrEmpty(component.NameAr))
            {
                return Json(new { success = false, message = "الاسم مطلوب" });
            }

            _context.SalaryComponents.Add(component);
            _context.HRLogs.Add(new HRLog
            {
                Action = $"User ({User.Identity.Name}) added salary component ({component.Name} / {component.NameAr})"
            });
            _context.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost]
        [Route("/payroll/salary-components/edit")]
        public IActionResult EditSalaryComponent(SalaryComponent updated)
        {
            var component = _context.SalaryComponents.Find(updated.Id);
            if (component == null)
            {
                return Json(new { success = false, message = "المكون غير موجود" });
            }

            component.Name = updated.Name;
            component.NameAr = updated.NameAr;
            component.Type = updated.Type;
            component.CalculationMethod = updated.CalculationMethod;
            component.DefaultAmount = updated.DefaultAmount;
            component.IsActive = updated.IsActive;
            component.Description = updated.Description;

            _context.HRLogs.Add(new HRLog
            {
                Action = $"User ({User.Identity.Name}) edited salary component ({component.Name})"
            });
            _context.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost]
        [Route("/payroll/salary-components/toggle")]
        public IActionResult ToggleSalaryComponent(int id)
        {
            var component = _context.SalaryComponents.Find(id);
            if (component == null)
            {
                return Json(new { success = false, message = "المكون غير موجود" });
            }

            component.IsActive = !component.IsActive;
            _context.HRLogs.Add(new HRLog
            {
                Action = $"User ({User.Identity.Name}) {(component.IsActive ? "activated" : "deactivated")} salary component ({component.Name})"
            });
            _context.SaveChanges();

            return Json(new { success = true, isActive = component.IsActive });
        }

        [HttpGet]
        [Route("/payroll/salary-components/get")]
        public IActionResult GetSalaryComponent(int id)
        {
            var component = _context.SalaryComponents.Find(id);
            if (component == null)
            {
                return Json(new { success = false });
            }
            return Json(new { success = true, data = component });
        }

        [HttpGet]
        [Route("/payroll/salary-components/active")]
        public IActionResult GetActiveComponents()
        {
            var components = _context.SalaryComponents
                .Where(c => c.IsActive)
                .OrderBy(c => c.Type)
                .ThenBy(c => c.Name)
                .Select(c => new { c.Id, c.Name, c.NameAr, c.Type, c.CalculationMethod, c.DefaultAmount })
                .ToList();
            return Json(components);
        }

        // ============ EMPLOYEE SALARY MANAGEMENT ============

        [HttpGet]
        [Route("/payroll/salaries")]
        public IActionResult Salaries()
        {
            var employees = _context.HREmployees
                .OrderBy(e => e.Name)
                .Select(e => new { e.Id, e.Name, e.JobName })
                .ToList();
            return View(employees);
        }

        [HttpGet]
        [Route("/payroll/salaries/get-employee")]
        public IActionResult GetEmployeeSalaries(int employeeId)
        {
            var salaries = _context.EmployeeSalaries
                .Where(s => s.EmployeeId == employeeId)
                .Select(s => new
                {
                    s.Id,
                    s.Amount,
                    s.IsActive,
                    s.EffectiveDate,
                    s.Notes,
                    ComponentName = s.SalaryComponent.Name,
                    ComponentNameAr = s.SalaryComponent.NameAr,
                    ComponentType = s.SalaryComponent.Type,
                    ComponentCalcMethod = s.SalaryComponent.CalculationMethod,
                    ComponentId = s.SalaryComponentId
                })
                .OrderBy(s => s.ComponentType)
                .ThenBy(s => s.ComponentName)
                .ToList();

            var employee = _context.HREmployees
                .Where(e => e.Id == employeeId)
                .Select(e => new { e.Name, e.JobName })
                .FirstOrDefault();

            return Json(new { success = true, salaries, employee });
        }

        [HttpPost]
        [Route("/payroll/salaries/add")]
        public IActionResult AddEmployeeSalary(int employeeId, int salaryComponentId, decimal amount, string? notes)
        {
            // Check if this component already exists for this employee
            var existing = _context.EmployeeSalaries
                .FirstOrDefault(s => s.EmployeeId == employeeId && s.SalaryComponentId == salaryComponentId && s.IsActive);

            if (existing != null)
            {
                return Json(new { success = false, message = "هذا المكون موجود بالفعل للموظف" });
            }

            var salary = new EmployeeSalary
            {
                EmployeeId = employeeId,
                SalaryComponentId = salaryComponentId,
                Amount = amount,
                Notes = notes,
                IsActive = true,
                EffectiveDate = DateTime.Now
            };

            _context.EmployeeSalaries.Add(salary);

            // Add salary history
            _context.SalaryHistories.Add(new SalaryHistory
            {
                EmployeeId = employeeId,
                SalaryComponentId = salaryComponentId,
                PreviousValue = null,
                NewValue = amount,
                EffectiveDate = DateTime.Now,
                Reason = "إضافة مكون راتب جديد",
                ChangedBy = User.Identity?.Name ?? "System",
                CreatedDate = DateTime.Now
            });

            _context.HRLogs.Add(new HRLog
            {
                Action = $"User ({User.Identity.Name}) added salary component ({salaryComponentId}) for employee ({employeeId}) amount ({amount})"
            });
            _context.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost]
        [Route("/payroll/salaries/edit")]
        public IActionResult EditEmployeeSalary(int id, decimal amount, string? notes)
        {
            var salary = _context.EmployeeSalaries.Find(id);
            if (salary == null)
            {
                return Json(new { success = false, message = "المكون غير موجود" });
            }

            var previousValue = salary.Amount;
            salary.Amount = amount;
            salary.Notes = notes;

            _context.SalaryHistories.Add(new SalaryHistory
            {
                EmployeeId = salary.EmployeeId,
                SalaryComponentId = salary.SalaryComponentId,
                PreviousValue = previousValue,
                NewValue = amount,
                EffectiveDate = DateTime.Now,
                Reason = "تعديل مكون راتب",
                ChangedBy = User.Identity?.Name ?? "System",
                CreatedDate = DateTime.Now
            });

            _context.HRLogs.Add(new HRLog
            {
                Action = $"User ({User.Identity.Name}) edited employee salary (id:{id}) from ({previousValue}) to ({amount})"
            });
            _context.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost]
        [Route("/payroll/salaries/toggle")]
        public IActionResult ToggleEmployeeSalary(int id)
        {
            var salary = _context.EmployeeSalaries.Find(id);
            if (salary == null)
            {
                return Json(new { success = false, message = "المكون غير موجود" });
            }

            salary.IsActive = !salary.IsActive;

            _context.HRLogs.Add(new HRLog
            {
                Action = $"User ({User.Identity.Name}) {(salary.IsActive ? "activated" : "deactivated")} employee salary (id:{id})"
            });
            _context.SaveChanges();

            return Json(new { success = true, isActive = salary.IsActive });
        }

        // ============ PAYROLL GENERATION & MANAGEMENT ============

        [HttpGet]
        [Route("/payroll/list")]
        public IActionResult PayrollList()
        {
            var payrolls = _context.Payrolls
                .OrderByDescending(p => p.Year)
                .ThenByDescending(p => p.Month)
                .Select(p => new
                {
                    p.Id,
                    p.Month,
                    p.Year,
                    p.Status,
                    p.GeneratedDate,
                    p.GeneratedBy,
                    p.Notes,
                    EmployeeCount = p.PayrollDetails.Count
                })
                .ToList()
                .Select(p => new PayrollListViewModel
                {
                    Id = p.Id,
                    Month = p.Month,
                    Year = p.Year,
                    MonthName = new System.Globalization.CultureInfo("ar-SA").DateTimeFormat.GetMonthName(p.Month),
                    Status = p.Status,
                    GeneratedDate = p.GeneratedDate,
                    GeneratedBy = p.GeneratedBy ?? "",
                    Notes = p.Notes,
                    EmployeeCount = p.EmployeeCount
                })
                .ToList();

            return View(payrolls);
        }

        [HttpGet]
        [Route("/payroll/generate")]
        public IActionResult GeneratePayroll()
        {
            ViewBag.Months = Enumerable.Range(1, 12).Select(m => new
            {
                Value = m,
                Text = new System.Globalization.CultureInfo("ar-SA").DateTimeFormat.GetMonthName(m)
            }).ToList();

            ViewBag.Years = Enumerable.Range(DateTime.Now.Year - 2, 5).ToList();
            return View();
        }

        [HttpPost]
        [Route("/payroll/generate/run")]
        public IActionResult RunPayrollGeneration(int month, int year)
        {
            // Check if payroll already exists for this month/year
            var existing = _context.Payrolls.FirstOrDefault(p => p.Month == month && p.Year == year);
            if (existing != null)
            {
                return Json(new { success = false, message = "تم إنشاء كشف راتب لهذا الشهر بالفعل" });
            }

            var userName = User.Identity?.Name ?? "System";
            var employees = _context.HREmployees.ToList();

            // Create the payroll header
            var payroll = new Payroll
            {
                Month = month,
                Year = year,
                Status = "Draft",
                GeneratedDate = DateTime.Now,
                GeneratedBy = userName
            };
            _context.Payrolls.Add(payroll);
            _context.SaveChanges();

            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var workingDays = (endDate - startDate).Days + 1;

            foreach (var emp in employees)
            {
                // Get active salary components
                var salaryComponents = _context.EmployeeSalaries
                    .Where(s => s.EmployeeId == emp.Id && s.IsActive)
                    .Include(s => s.SalaryComponent)
                    .ToList();

                var basicSalary = salaryComponents
                    .FirstOrDefault(s => s.SalaryComponent.Name == "Basic Salary")?.Amount ?? 0;

                var totalEarnings = salaryComponents
                    .Where(s => s.SalaryComponent.Type == "Earning")
                    .Sum(s => s.Amount);

                var totalDeductions = salaryComponents
                    .Where(s => s.SalaryComponent.Type == "Deduction")
                    .Sum(s => s.Amount);

                // Attendance stats from Basma
                var attendanceRecords = _context.HREmployeeBasmas
                    .Where(b => b.EmployeeId == emp.Id
                             && b.DayDate >= startDate
                             && b.DayDate <= endDate)
                    .ToList();

                var presentDays = attendanceRecords.Count(b => b.Status == 1);
                var absentDays = attendanceRecords.Count(b => b.Status == 0 || b.Status == 3);
                var lateMinutes = attendanceRecords.Sum(b => b.LateMinutes ?? 0);
                var overtimeHours = attendanceRecords.Sum(b => b.OvertimeMinutes ?? 0) / 60f;

                // Leave stats
                var paidLeaves = attendanceRecords.Count(b => b.OffDayType == "annual" || b.OffDayType == "sick");
                var unpaidLeaves = attendanceRecords.Count(b => b.OffDayType == "unpaid");
                var officialHolidays = attendanceRecords.Count(b => b.OffDayType == "official");

                // Daily salary rate
                var dailySalaryRate = workingDays > 0 ? basicSalary / workingDays : 0;

                var grossSalary = totalEarnings;
                var netSalary = grossSalary - totalDeductions;

                var detail = new PayrollDetail
                {
                    PayrollId = payroll.Id,
                    EmployeeId = emp.Id,
                    BasicSalary = basicSalary,
                    TotalEarnings = totalEarnings,
                    TotalDeductions = totalDeductions,
                    GrossSalary = grossSalary,
                    NetSalary = netSalary,
                    PresentDays = presentDays,
                    AbsentDays = absentDays,
                    LateMinutes = lateMinutes,
                    OvertimeHours = overtimeHours,
                    PaidLeaves = paidLeaves,
                    UnpaidLeaves = unpaidLeaves,
                    OfficialHolidays = officialHolidays,
                    DailySalaryRate = dailySalaryRate
                };
                _context.PayrollDetails.Add(detail);
                _context.SaveChanges();

                // Create individual earning records
                foreach (var comp in salaryComponents.Where(s => s.SalaryComponent.Type == "Earning"))
                {
                    _context.PayrollEarnings.Add(new PayrollEarning
                    {
                        PayrollDetailId = detail.Id,
                        SalaryComponentId = comp.SalaryComponentId,
                        Name = comp.SalaryComponent.NameAr,
                        Amount = comp.Amount
                    });
                }

                // Create individual deduction records
                foreach (var comp in salaryComponents.Where(s => s.SalaryComponent.Type == "Deduction"))
                {
                    _context.PayrollDeductions.Add(new PayrollDeduction
                    {
                        PayrollDetailId = detail.Id,
                        SalaryComponentId = comp.SalaryComponentId,
                        Name = comp.SalaryComponent.NameAr,
                        Amount = comp.Amount
                    });
                }
            }

            _context.HRLogs.Add(new HRLog
            {
                Action = $"User ({userName}) generated payroll for {month}/{year}"
            });
            _context.SaveChanges();

            return Json(new { success = true, payrollId = payroll.Id });
        }

        [HttpGet]
        [Route("/payroll/details/{id}")]
        public IActionResult PayrollDetails(int id)
        {
            var payroll = _context.Payrolls
                .Where(p => p.Id == id)
                .Select(p => new
                {
                    p.Id,
                    p.Month,
                    p.Year,
                    p.Status,
                    p.GeneratedDate,
                    p.GeneratedBy,
                    p.ReviewedDate,
                    p.ReviewedBy,
                    p.ApprovedDate,
                    p.ApprovedBy,
                    p.Notes
                })
                .FirstOrDefault();

            if (payroll == null) return NotFound();

            var details = _context.PayrollDetails
                .Where(d => d.PayrollId == id)
                .Select(d => new
                {
                    d.Id,
                    EmployeeName = d.HREmployee.Name,
                    d.BasicSalary,
                    d.TotalEarnings,
                    d.TotalDeductions,
                    d.GrossSalary,
                    d.NetSalary,
                    d.PresentDays,
                    d.AbsentDays,
                    d.LateMinutes,
                    d.OvertimeHours,
                    d.PaidLeaves,
                    d.UnpaidLeaves,
                    d.DailySalaryRate,
                    d.Notes
                })
                .ToList();

            var totalBasic = details.Sum(d => d.BasicSalary);
            var totalEarnings = details.Sum(d => d.TotalEarnings);
            var totalDeductions = details.Sum(d => d.TotalDeductions);
            var totalGross = details.Sum(d => d.GrossSalary);
            var totalNet = details.Sum(d => d.NetSalary);

            ViewBag.Payroll = payroll;
            ViewBag.TotalBasic = totalBasic;
            ViewBag.TotalEarnings = totalEarnings;
            ViewBag.TotalDeductions = totalDeductions;
            ViewBag.TotalGross = totalGross;
            ViewBag.TotalNet = totalNet;
            return View(details);
        }

        [HttpPost]
        [Route("/payroll/update-status")]
        public IActionResult UpdatePayrollStatus(int id, string status)
        {
            var payroll = _context.Payrolls.Find(id);
            if (payroll == null)
            {
                return Json(new { success = false, message = "كشف الراتب غير موجود" });
            }

            if (payroll.Status == "Locked")
            {
                return Json(new { success = false, message = "لا يمكن تعديل كشف راتب مقفل" });
            }

            var userName = User.Identity?.Name ?? "System";
            payroll.Status = status;

            if (status == "Reviewed")
            {
                payroll.ReviewedDate = DateTime.Now;
                payroll.ReviewedBy = userName;
            }
            else if (status == "Approved")
            {
                payroll.ApprovedDate = DateTime.Now;
                payroll.ApprovedBy = userName;
            }

            _context.HRLogs.Add(new HRLog
            {
                Action = $"User ({userName}) updated payroll ({id}) status to ({status})"
            });
            _context.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost]
        [Route("/payroll/update-notes")]
        public IActionResult UpdatePayrollNotes(int id, string notes)
        {
            var payroll = _context.Payrolls.Find(id);
            if (payroll == null)
            {
                return Json(new { success = false, message = "كشف الراتب غير موجود" });
            }

            payroll.Notes = notes;
            _context.SaveChanges();
            return Json(new { success = true });
        }

        // ============ SALARY HISTORY ============

        [HttpGet]
        [Route("/payroll/salary-history")]
        public IActionResult SalaryHistory()
        {
            var employees = _context.HREmployees
                .OrderBy(e => e.Name)
                .Select(e => new { e.Id, e.Name })
                .ToList();
            return View(employees);
        }

        [HttpGet]
        [Route("/payroll/salary-history/get")]
        public IActionResult GetSalaryHistory(int employeeId)
        {
            var history = _context.SalaryHistories
                .Where(h => h.EmployeeId == employeeId)
                .OrderByDescending(h => h.CreatedDate)
                .Select(h => new
                {
                    h.Id,
                    h.PreviousValue,
                    h.NewValue,
                    h.EffectiveDate,
                    h.Reason,
                    h.ChangedBy,
                    h.CreatedDate,
                    ComponentName = h.SalaryComponent != null ? h.SalaryComponent.NameAr : "جميع المكونات"
                })
                .ToList();

            return Json(new { success = true, history });
        }
    }
}
