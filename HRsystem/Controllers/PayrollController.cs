using HRsystem.Data;
using HRsystem.Helpers;
using HRsystem.Models;
using HRsystem.Models.Enums;
using HRsystem.ViewModels;
using HRsystem.Services;
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
        private readonly PayrollCalculationService _payrollCalculationService;
        private readonly PenaltyCalculationService _penaltyCalculationService;

        public PayrollController(
            ILogger<PayrollController> logger,
            AppDbContext context,
            PayrollCalculationService payrollCalculationService,
            PenaltyCalculationService penaltyCalculationService)
        {
            _logger = logger;
            _context = context;
            _payrollCalculationService = payrollCalculationService;
            _penaltyCalculationService = penaltyCalculationService;
        }

        // ============ PAYROLL INDEX (Landing Page) ============

        [HttpGet]
        [Route("/payroll")]
        public IActionResult Index()
        {
            return View();
        }

        // ============ PLACE SALARIES (وضع الرواتب) ============

        [HttpGet]
        [Route("/payroll/place-salaries")]
        public IActionResult PlaceSalaries()
        {
            var employees = _context.HREmployees
                .OrderBy(e => e.Name)
                .Select(e => new
                {
                    e.Id,
                    e.Name,
                    e.PhoneNumber,
                    e.ContractType,
                    e.NationalId,
                    e.JobName,
                    e.MarriageStatus,
                    e.Religion,
                    e.DateOfBirth,
                    e.InsuranceNumber,
                    e.Address,
                    e.HireDate,
                    e.EndDate,
                    e.LeaveReason,
                    e.BasmaId,
                    e.HRDepartmentId
                })
                .ToList();
            return View(employees);
        }

        [HttpGet]
        [Route("/payroll/place-salaries/get-employee-detail")]
        public IActionResult GetEmployeeDetail(int employeeId)
        {
            var emp = _context.HREmployees
                .Where(e => e.Id == employeeId)
                .Select(e => new
                {
                    e.Id,
                    e.Name,
                    e.NationalId,
                    e.PhoneNumber,
                    e.MarriageStatus,
                    e.Religion,
                    e.DateOfBirth,
                    e.InsuranceNumber,
                    e.Address,
                    e.HireDate,
                    e.EndDate,
                    e.LeaveReason,
                    e.JobName,
                    e.ContractType,
                    e.BasmaId,
                    DepartmentName = e.HRDepartment != null ? e.HRDepartment.Name : null
                })
                .FirstOrDefault();

            if (emp == null)
                return Json(new { success = false });

            return Json(new { success = true, data = emp });
        }

        // ============ SALARY COMPONENTS ============

        [HttpGet]
        [Route("/payroll/salary-components")]
        public IActionResult SalaryComponents()
        {
            var components = _context.PayrollComponents
                .OrderBy(c => c.Category)
                .ThenBy(c => c.Name)
                .ToList();
            return View(components);
        }

        [HttpPost]
        [Route("/payroll/salary-components/add")]
        public IActionResult AddSalaryComponent(PayrollComponent component)
        {
            if (string.IsNullOrEmpty(component.Name) || string.IsNullOrEmpty(component.NameAr))
            {
                return Json(new { success = false, message = "الاسم مطلوب" });
            }
            if (string.IsNullOrEmpty(component.Code))
            {
                component.Code = component.Name.Replace(" ", "_").ToUpper();
            }

            if (_context.PayrollComponents.Any(pc => pc.Code == component.Code))
            {
                return Json(new { success = false, message = $"الكود ({component.Code}) مستخدم من قبل بالفعل" });
            }

            component.CreatedAt = DateTime.Now;
            _context.PayrollComponents.Add(component);
            _context.HRLogs.Add(new HRLog
            {
                Action = $"User ({User.Identity.Name}) added payroll component ({component.Name} / {component.NameAr})"
            });
            _context.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost]
        [Route("/payroll/salary-components/edit")]
        public IActionResult EditSalaryComponent(PayrollComponent updated)
        {
            var component = _context.PayrollComponents.Find(updated.Id);
            if (component == null)
            {
                return Json(new { success = false, message = "المكون غير موجود" });
            }

            component.Name = updated.Name;
            component.NameAr = updated.NameAr;
            component.Code = updated.Code;
            component.Category = updated.Category;
            component.CalculationMethod = updated.CalculationMethod;
            component.DefaultAmount = updated.DefaultAmount;
            component.DefaultPercentage = updated.DefaultPercentage;
            component.IsActive = updated.IsActive;
            component.IsTaxable = updated.IsTaxable;
            component.IsInsurable = updated.IsInsurable;
            component.IsRecurring = updated.IsRecurring;
            component.SortOrder = updated.SortOrder;
            component.Description = updated.Description;

            _context.HRLogs.Add(new HRLog
            {
                Action = $"User ({User.Identity.Name}) edited payroll component ({component.Name})"
            });
            _context.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost]
        [Route("/payroll/salary-components/toggle")]
        public IActionResult ToggleSalaryComponent(int id)
        {
            var component = _context.PayrollComponents.Find(id);
            if (component == null)
            {
                return Json(new { success = false, message = "المكون غير موجود" });
            }

            component.IsActive = !component.IsActive;
            _context.HRLogs.Add(new HRLog
            {
                Action = $"User ({User.Identity.Name}) {(component.IsActive ? "activated" : "deactivated")} payroll component ({component.Name})"
            });
            _context.SaveChanges();

            return Json(new { success = true, isActive = component.IsActive });
        }

        [HttpGet]
        [Route("/payroll/salary-components/get")]
        public IActionResult GetSalaryComponent(int id)
        {
            var component = _context.PayrollComponents.Find(id);
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
            var components = _context.PayrollComponents
                .Where(c => c.IsActive)
                .OrderBy(c => c.Category)
                .ThenBy(c => c.Name)
                .Select(c => new { c.Id, c.Name, c.NameAr, c.Code, c.Category, c.CalculationMethod, c.DefaultAmount, c.DefaultPercentage })
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
            var salaries = _context.EmployeePayrollComponents
                .Where(s => s.EmployeeId == employeeId)
                .Select(s => new
                {
                    s.Id,
                    s.Amount,
                    s.IsActive,
                    s.StartDate,
                    s.EndDate,
                    s.Notes,
                    ComponentName = s.PayrollComponent.Name,
                    ComponentNameAr = s.PayrollComponent.NameAr,
                    ComponentCategory = s.PayrollComponent.Category,
                    ComponentCalcMethod = s.PayrollComponent.CalculationMethod,
                    ComponentId = s.PayrollComponentId
                })
                .OrderBy(s => s.ComponentCategory)
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
        public IActionResult AddEmployeeSalary(int employeeId, int payrollComponentId, decimal amount, string? notes)
        {
            var existing = _context.EmployeePayrollComponents
                .FirstOrDefault(s => s.EmployeeId == employeeId && s.PayrollComponentId == payrollComponentId && s.IsActive);

            if (existing != null)
            {
                return Json(new { success = false, message = "هذا المكون موجود بالفعل للموظف" });
            }

            var salary = new EmployeePayrollComponent
            {
                EmployeeId = employeeId,
                PayrollComponentId = payrollComponentId,
                Amount = amount,
                Notes = notes,
                IsActive = true,
                StartDate = DateTime.Now
            };

            _context.EmployeePayrollComponents.Add(salary);
            _context.SaveChanges();

            var currentUserName = User.Identity?.Name ?? "";
            var changedByUser = _context.Users.FirstOrDefault(u => u.Username == currentUserName);
            _context.PayrollComponentHistories.Add(new PayrollComponentHistory
            {
                EmployeeId = employeeId,
                PayrollComponentId = payrollComponentId,
                OldAmount = null,
                NewAmount = amount,
                EffectiveDate = DateTime.Now,
                Reason = "إضافة مكون راتب جديد",
                ChangedByUserId = changedByUser?.Id,
                CreatedAt = DateTime.Now
            });

            _context.HRLogs.Add(new HRLog
            {
                Action = $"User ({User.Identity.Name}) added payroll component ({payrollComponentId}) for employee ({employeeId}) amount ({amount})"
            });
            _context.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost]
        [Route("/payroll/salaries/edit")]
        public IActionResult EditEmployeeSalary(int id, decimal amount, string? notes)
        {
            var salary = _context.EmployeePayrollComponents.Find(id);
            if (salary == null)
            {
                return Json(new { success = false, message = "المكون غير موجود" });
            }

            var previousValue = salary.Amount;
            salary.Amount = amount;
            salary.Notes = notes;

            var currentUserName = User.Identity?.Name ?? "";
            var changedByUser = _context.Users.FirstOrDefault(u => u.Username == currentUserName);
            _context.PayrollComponentHistories.Add(new PayrollComponentHistory
            {
                EmployeeId = salary.EmployeeId,
                PayrollComponentId = salary.PayrollComponentId,
                OldAmount = previousValue,
                NewAmount = amount,
                EffectiveDate = DateTime.Now,
                Reason = "تعديل مكون راتب",
                ChangedByUserId = changedByUser?.Id,
                CreatedAt = DateTime.Now
            });

            _context.HRLogs.Add(new HRLog
            {
                Action = $"User ({User.Identity.Name}) edited employee salary (id:{id}) from ({previousValue}) to ({amount})"
            });
            _context.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost]
        [Route("/payroll/salaries/delete")]
        public IActionResult DeleteEmployeeSalary(int id)
        {
            var salary = _context.EmployeePayrollComponents.Find(id);
            if (salary == null)
            {
                return Json(new { success = false, message = "المكون غير موجود" });
            }

            _context.EmployeePayrollComponents.Remove(salary);

            _context.HRLogs.Add(new HRLog
            {
                Action = $"User ({User.Identity.Name}) deleted employee salary (id:{id}) for employee ({salary.EmployeeId})"
            });
            _context.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost]
        [Route("/payroll/salaries/toggle")]
        public IActionResult ToggleEmployeeSalary(int id)
        {
            var salary = _context.EmployeePayrollComponents.Find(id);
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
                    EmployeeCount = p.PayrollItems.Count
                })
                .ToList()
                .Select(p => new PayrollListViewModel
                {
                    Id = p.Id,
                    Month = p.Month,
                    Year = p.Year,
                    MonthName = MonthNamesHelper.GetGregorianMonthName(p.Month),
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
                Text = MonthNamesHelper.GetGregorianMonthName(m)
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
            var selectedPolicy = _context.PayrollPolicies.FirstOrDefault(p => p.IsActive);
            var payroll = new Payroll
            {
                Month = month,
                Year = year,
                Status = PayrollStatus.Draft,
                GeneratedDate = DateTime.Now,
                GeneratedBy = userName,
                PayrollPolicyId = selectedPolicy?.Id
            };
            _context.Payrolls.Add(payroll);
            _context.SaveChanges();

            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var policy = _context.PayrollPolicies
                .Include(p => p.TaxBrackets)
                .Include(p => p.InsurancePolicies)
                .FirstOrDefault(p => p.IsActive)
                ?? new PayrollPolicy();
            var workingDays = policy.DailySalaryCalcMethod == DailySalaryCalcMethod.CalendarDays
                ? (decimal)DateTime.DaysInMonth(year, month)
                : policy.WorkingDaysPerMonth;
            var workingHoursPerDay = policy.WorkingHoursPerDay;

            // Payroll component used to post approved disciplinary penalties (الجزاءات المعتمدة) as salary deductions
            var penaltyComponent = _context.PayrollComponents
                .FirstOrDefault(pc => pc.Code == "PENALTY");

            foreach (var emp in employees)
            {
                // Get active payroll components assigned to this employee
                var salaryComponents = _context.EmployeePayrollComponents
                    .Where(s => s.EmployeeId == emp.Id && s.IsActive)
                    .Include(s => s.PayrollComponent)
                    .ToList();

                var basicComponent = salaryComponents
                    .FirstOrDefault(s => s.PayrollComponent.Category == PayrollComponentCategory.Salary);
                var basicSalary = basicComponent?.Amount ?? 0;

                var earningComponents = salaryComponents
                    .Where(s => s.PayrollComponent.Category != PayrollComponentCategory.Deduction)
                    .ToList();
                var deductionComponents = salaryComponents
                    .Where(s => s.PayrollComponent.Category == PayrollComponentCategory.Deduction)
                    .ToList();

                var totalEarnings = earningComponents.Sum(s => s.Amount);
                var totalDeductions = deductionComponents.Sum(s => s.Amount);

                // Attendance stats from Basma
                var attendanceRecords = _context.HREmployeeBasmas
                    .Where(b => b.EmployeeId == emp.Id
                             && b.DayDate >= startDate
                             && b.DayDate <= endDate)
                    .ToList();

                var presentDays = attendanceRecords.Count(b => b.Status == 1);
                var absentDays = attendanceRecords.Count(b => b.Status == 0 || b.Status == 3);
                var lateMinutes = attendanceRecords.Sum(b => b.LateMinutes ?? 0);
                var overtimeHours = attendanceRecords.Sum(b => b.OvertimeMinutes ?? 0) / 60.0;

                // Leave stats
                var paidLeaves = attendanceRecords.Count(b => b.OffDayType == "annual" || b.OffDayType == "sick");
                var unpaidLeaves = attendanceRecords.Count(b => b.OffDayType == "unpaid");
                var officialHolidays = attendanceRecords.Count(b => b.OffDayType == "official");

                // Daily salary rate
                var dailySalaryRate = _payrollCalculationService.CalculateDailySalaryRate(
                    basicSalary, policy, workingDays);

                // ===== Approved Penalties → auto salary deductions =====
                // Only approved penalties that belong to this month and were never
                // posted to a payroll before (PayrollItemId == null) are converted.
                var approvedPenalties = _context.EmployeePenalties
                    .Where(p => p.EmployeeId == emp.Id
                             && p.Status == PenaltyStatus.Approved
                             && p.PayrollItemId == null
                             && p.IncidentDate >= startDate
                             && p.IncidentDate <= endDate)
                    .Include(p => p.PenaltyRule)
                    .ToList();

                var penaltyEntries = new List<(EmployeePenalty Penalty, PayrollItem Item, decimal Amount, decimal Days)>();
                if (penaltyComponent != null)
                {
                    foreach (var penalty in approvedPenalties)
                    {
                        if (penalty.DeductionUnit == DeductionUnit.WarningOnly)
                            continue;

                        var penaltyResult = _penaltyCalculationService.Calculate(new(
                            penalty.DeductionUnit,
                            penalty.DeductionValue,
                            basicSalary,
                            dailySalaryRate,
                            workingHoursPerDay));
                        var penaltyAmount = penaltyResult.Amount;
                        var penaltyDays = penaltyResult.DeductionDays ?? 0m;

                        if (penaltyAmount <= 0)
                            continue;

                        totalDeductions += penaltyAmount;

                        penaltyEntries.Add((penalty, new PayrollItem
                        {
                            PayrollId = payroll.Id,
                            EmployeeId = emp.Id,
                            PayrollComponentId = penaltyComponent.Id,
                            Amount = penaltyAmount,
                            Quantity = penalty.DeductionValue,
                            Rate = dailySalaryRate,
                            IsManual = false,
                            IsActive = true,
                            SourceType = PayrollItemSourceType.Penalty,
                            SourceId = penalty.Id,
                            Notes = penalty.PenaltyRule?.NameAr ?? "خصم جزائي"
                        }, penaltyAmount, penaltyDays));
                    }
                }

                // Calculate Taxable & Insurable amounts from component flags
                var taxableAmount = earningComponents
                    .Where(s => s.PayrollComponent.IsTaxable)
                    .Sum(s => s.Amount);

                var insurableAmount = earningComponents
                    .Where(s => s.PayrollComponent.IsInsurable)
                    .Sum(s => s.Amount);

                var insurancePolicy = policy.InsurancePolicies
                    .Where(i => i.IsActive && i.EffectiveDate <= endDate)
                    .OrderByDescending(i => i.EffectiveDate).FirstOrDefault();
                var calculation = _payrollCalculationService.Calculate(new(
                    basicSalary,
                    totalEarnings,
                    totalDeductions,
                    taxableAmount,
                    insurableAmount,
                    policy,
                    policy.TaxBrackets.Where(b => b.IsActive && b.EffectiveDate <= endDate).ToList(),
                    insurancePolicy));

                var detail = new PayrollDetail
                {
                    PayrollId = payroll.Id,
                    EmployeeId = emp.Id,
                    BasicSalary = basicSalary,
                    TotalEarnings = totalEarnings,
                    TotalDeductions = calculation.TotalDeductions,
                    GrossSalary = calculation.GrossSalary,
                    NetSalary = calculation.NetSalary,
                    TaxableAmount = taxableAmount,
                    InsurableAmount = insurableAmount,
                    TaxAmount = calculation.TaxAmount,
                    InsuranceAmount = calculation.InsuranceAmount,
                    PresentDays = presentDays,
                    AbsentDays = absentDays,
                    LateMinutes = lateMinutes,
                    OvertimeHours = overtimeHours,
                    PaidLeaves = paidLeaves,
                    UnpaidLeaves = unpaidLeaves,
                    OfficialHolidays = officialHolidays,
                    DailySalaryRate = calculation.DailySalaryRate
                };
                _context.PayrollDetails.Add(detail);
                _context.SaveChanges();

                // Create payroll item rows (new source of truth) + rollup rows (kept detail tables)
                foreach (var comp in salaryComponents)
                {
                    _context.PayrollItems.Add(new PayrollItem
                    {
                        PayrollId = payroll.Id,
                        EmployeeId = emp.Id,
                        PayrollComponentId = comp.PayrollComponentId,
                        Amount = comp.Amount,
                        IsManual = false,
                        IsActive = true,
                        SourceType = PayrollItemSourceType.Recurring,
                        Notes = comp.Notes
                    });

                    if (comp.PayrollComponent.Category != PayrollComponentCategory.Deduction)
                    {
                        _context.PayrollEarnings.Add(new PayrollEarning
                        {
                            PayrollDetailId = detail.Id,
                            Name = comp.PayrollComponent.NameAr,
                            Amount = comp.Amount,
                            IsTaxable = comp.PayrollComponent.IsTaxable,
                            IsInsurable = comp.PayrollComponent.IsInsurable
                        });
                    }
                    else
                    {
                        _context.PayrollDeductions.Add(new PayrollDeduction
                        {
                            PayrollDetailId = detail.Id,
                            Name = comp.PayrollComponent.NameAr,
                            Amount = comp.Amount,
                            IsTaxable = comp.PayrollComponent.IsTaxable,
                            IsInsurable = comp.PayrollComponent.IsInsurable
                        });
                    }
                }

                // ===== Post approved penalties as deduction rows + PayrollItems =====
                if (penaltyEntries.Count > 0)
                {
                    foreach (var (penalty, item, amount, days) in penaltyEntries)
                    {
                        _context.PayrollItems.Add(item);
                        _context.PayrollDeductions.Add(new PayrollDeduction
                        {
                            PayrollDetailId = detail.Id,
                            Name = penalty.PenaltyRule?.NameAr ?? "خصم جزائي",
                            Amount = amount,
                            IsTaxable = false,
                            IsInsurable = false
                        });
                    }

                    // Save now to materialize PayrollItem.Id, then link each penalty back to its item
                    _context.SaveChanges();
                    foreach (var (penalty, item, amount, days) in penaltyEntries)
                    {
                        penalty.PayrollItemId = item.Id;
                        penalty.DeductionAmount = amount;
                        penalty.DeductionDays = (penalty.DeductionUnit == DeductionUnit.Day || penalty.DeductionUnit == DeductionUnit.Hour)
                            ? days
                            : null;
                    }
                }
            }

            _context.HRLogs.Add(new HRLog
            {
                Action = $"User ({userName}) generated payroll for {month}/{year}"
            });
            _context.SaveChanges();

            return Json(new { success = true, payrollId = payroll.Id });
        }

        private static decimal CalculateProgressiveTax(decimal taxableAmount, List<TaxBracket> brackets)
        {
            return PayrollCalculationService.CalculateProgressiveTax(taxableAmount, brackets);
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
                    d.TaxableAmount,
                    d.InsurableAmount,
                    d.TaxAmount,
                    d.InsuranceAmount,
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
            var totalTaxable = details.Sum(d => d.TaxableAmount);
            var totalInsurable = details.Sum(d => d.InsurableAmount);
            var totalTax = details.Sum(d => d.TaxAmount);
            var totalInsurance = details.Sum(d => d.InsuranceAmount);

            ViewBag.Payroll = payroll;
            ViewBag.TotalBasic = totalBasic;
            ViewBag.TotalEarnings = totalEarnings;
            ViewBag.TotalDeductions = totalDeductions;
            ViewBag.TotalGross = totalGross;
            ViewBag.TotalNet = totalNet;
            ViewBag.TotalTaxable = totalTaxable;
            ViewBag.TotalInsurable = totalInsurable;
            ViewBag.TotalTax = totalTax;
            ViewBag.TotalInsurance = totalInsurance;
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

            if (payroll.Status == PayrollStatus.Locked)
            {
                return Json(new { success = false, message = "لا يمكن تعديل كشف راتب مقفل" });
            }

            var userName = User.Identity?.Name ?? "System";
            payroll.Status = Enum.Parse<PayrollStatus>(status);

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
            var history = _context.PayrollComponentHistories
                .Where(h => h.EmployeeId == employeeId)
                .OrderByDescending(h => h.CreatedAt)
                .Select(h => new
                {
                    h.Id,
                    PreviousValue = h.OldAmount,
                    NewValue = h.NewAmount,
                    h.EffectiveDate,
                    h.Reason,
                    ChangedBy = h.ChangedByUser != null ? h.ChangedByUser.Username : "System",
                    CreatedDate = h.CreatedAt,
                    ComponentName = h.PayrollComponent != null ? h.PayrollComponent.NameAr : "جميع المكونات"
                })
                .ToList();

            return Json(new { success = true, history });
        }
    }
}
