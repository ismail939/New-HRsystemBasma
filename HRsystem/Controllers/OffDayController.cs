using System.Diagnostics;
using HRsystem.Data;
using HRsystem.Models;
using HRsystem.Models.Enums;
using HRsystem.Services;
using HRsystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRsystem.Controllers
{
    public class OffDayController : Controller
    {
        private readonly ILogger<OffDayController> _logger;
        private readonly AppDbContext _context;
        private readonly IOffDayBalanceAutoService _balanceService;

        public OffDayController(
            ILogger<OffDayController> logger, 
            AppDbContext context,
            IOffDayBalanceAutoService balanceService)
        {
            _logger = logger;
            _context = context;
            _balanceService = balanceService;
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpGet]
        [Route("/OffDays")]
        public IActionResult OffDay()
        {
            Console.WriteLine("Entered ListEmployees action");
            var employees = _context.HREmployees.ToList();
            Console.WriteLine("here is the number of the items: " + employees.Count);
            foreach (var emp in employees)
            {
                Console.WriteLine(emp.Name + " - " + emp.HRDepartmentId);
            }
            var employeeVMs = new List<EmployeeViewModel>();
            foreach (var emp in employees)
            {
                Console.WriteLine($"Processing employee: {emp.Name} with department ID: {emp.HRDepartmentId}");
                var dep = _context.HRDepartments.FirstOrDefault(d => d.Id == emp.HRDepartmentId);
                string depName = dep != null ? dep.Name : "";
                employeeVMs.Add(new EmployeeViewModel
                {
                    Id = emp.Id,
                    Name = emp.Name,
                    NationalId = emp.NationalId,
                    PhoneNumber = emp.PhoneNumber,
                    MarriageStatus = emp.MarriageStatus,
                    Religion = emp.Religion,
                    DateOfBirth = emp.DateOfBirth ?? DateTime.MinValue,
                    InsuranceNumber = emp.InsuranceNumber,
                    HireDate = emp.HireDate,
                    EndDate = emp.EndDate,
                    JobName = emp.JobName,
                    ContractType = emp.ContractType,
                    LeaveReason = emp.LeaveReason,
                    BasmaId = emp.BasmaId,
                    HRDepartmentId = emp.HRDepartmentId,
                    Department = depName==""?"": depName
                });
            }
            foreach (var emp in employeeVMs)
            {
                Console.WriteLine(emp.Name + " - " + emp.Department);
            }
            return View(employeeVMs);
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpGet("employees/offdays/{employeeId}")]
        public IActionResult GetOffDays(
            int employeeId,
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            Console.WriteLine(string.Format("employeeId: {0}, from: {1}, to: {2}", employeeId, from, to));
            var list = _context.HREmployeeOffDays.Where(x => x.EmployeeId == employeeId &&
            x.OffDayDate >= from &&
            x.OffDayDate <= to
            ).OrderBy(x => x.OffDayDate).ToList();
            return Json(list);
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpPost]
        [Route("/employees/offdays/edit")]
        public async Task<IActionResult> Edit([FromBody] EditOffDaysRequest request)
        {
            foreach (var d in request.Days)
            {
                var date = DateTime.Parse(d.OffDayDate);
                if (d.IsOffDay)
                {
                    // check if there is an entry on the same day
                    var existing = _context.HREmployeeOffDays
                        .FirstOrDefault(x => x.EmployeeId == request.EmployeeId
                                         && x.OffDayDate.Date == date.Date);
                    if (existing != null)
                    {
                        _context.HREmployeeOffDays.Remove(existing);
                    }
                    // Add or update offday
                    _context.HREmployeeOffDays.Add(new HREmployeeOffDay
                    {
                        EmployeeId = request.EmployeeId,
                        OffDayDate = date,
                        OffDayType = d.OffDayType
                    });
                    
                    // Deduct 1 day from the corresponding balance (force deduct for HR)
                    var leaveType = MapArabicOffDayTypeToLeaveType(d.OffDayType);
                    if (leaveType != null)
                    {
                        await _balanceService.ForceDeductBalanceAsync(request.EmployeeId, leaveType.Value, 1);
                        _context.HRLogs.Add(new HRLog
                        {
                            Action = $"User ({User.Identity.Name}) manually added off day ({d.OffDayType}) for employeeId ({request.EmployeeId}) on {date:yyyy-MM-dd} - deducted 1 day from balance"
                        });
                    }
                }
                else
                {
                    // Remove from DB if exists
                    var existing = _context.HREmployeeOffDays
                        .FirstOrDefault(x => x.EmployeeId == request.EmployeeId
                                          && x.OffDayDate.Date == date.Date);

                    if (existing != null)
                    {
                        var removedType = existing.OffDayType; // save before removing
                        _context.HREmployeeOffDays.Remove(existing);
                        
                        // Add back 1 day to the corresponding balance
                        var leaveType = MapArabicOffDayTypeToLeaveType(removedType);
                        if (leaveType != null)
                        {
                            // Add back by calling a service method (or directly)
                            var balance = _context.HROffDayBalances
                                .FirstOrDefault(b => b.EmployeeId == request.EmployeeId);
                            if (balance != null)
                            {
                                AddToBalance(balance, leaveType.Value, 1);
                                _context.HRLogs.Add(new HRLog
                                {
                                    Action = $"User ({User.Identity.Name}) manually removed off day ({removedType}) for employeeId ({request.EmployeeId}) on {date:yyyy-MM-dd} - added back 1 day to balance"
                                });
                            }
                        }
                    }
                }
            }
            
            _context.SaveChanges();

            return Ok("Done");
        }
        
        private LeaveType? MapArabicOffDayTypeToLeaveType(string? arabicType)
        {
            if (string.IsNullOrEmpty(arabicType)) return null;
            return arabicType switch
            {
                "سنوي" => LeaveType.Annual,
                "عارضة" => LeaveType.Casual,
                "مرضي" => LeaveType.Sick,
                "حج" => LeaveType.Hajj,
                "أمومة" => LeaveType.Maternity,
                "بدون راتب" => LeaveType.Unpaid,
                "تعويضي" => LeaveType.Compensatory,
                "رسمية" => LeaveType.OfficialHoliday,
                "اختبارات" => LeaveType.Exam,
                "راحة" => LeaveType.Unpaid, // treat rest days as unpaid
                _ => null // "أخرى" or unknown types won't deduct
            };
        }
        
        private void AddToBalance(HROffDayBalance balance, LeaveType leaveType, int days)
        {
            switch (leaveType)
            {
                case LeaveType.Annual: balance.Annual += days; break;
                case LeaveType.Casual: balance.Casual += days; break;
                case LeaveType.Sick: balance.Sick += days; break;
                case LeaveType.Hajj: balance.Hajj += days; break;
                case LeaveType.Maternity: balance.Maternity += days; break;
                case LeaveType.Unpaid: balance.Unpaid += days; break;
                case LeaveType.Compensatory: balance.Compensatory += days; break;
                case LeaveType.OfficialHoliday: balance.OfficialHoliday += days; break;
                case LeaveType.Exam: balance.Exam += days; break;
            }
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpGet]
        [Route("/offdays/balance/add")]
        public async Task<IActionResult> AddOffDayBalance(int employeeId)
        {
            Console.WriteLine("Adding off day balance for employeeId:❎ " + employeeId);
            var existing = _context.HROffDayBalances
                .FirstOrDefault(x => x.EmployeeId == employeeId);
            if (existing != null)
            {
                Console.WriteLine("Off day balance already exists for employeeId:✅ " + employeeId);
                 return Json(new
            {
                annualBalance = existing.Annual,
                casualBalance = existing.Casual,
                sickBalance = existing.Sick,
                hajjBalance = existing.Hajj,
                maternityBalance = existing.Maternity,
                unpaidBalance = existing.Unpaid,
                compensatoryBalance = existing.Compensatory,
                officialHolidayBalance = existing.OfficialHoliday,
                examBalance = existing.Exam,
                isAutoCalculated = existing.IsAutoCalculated,
                notes = existing.Notes
            });
            }
            // Try auto-calculate first
            var balance = await _balanceService.CalculateAndSaveBalanceAsync(employeeId);
            
            return Json(new
            {
                annualBalance = balance.Annual,
                casualBalance = balance.Casual,
                sickBalance = balance.Sick,
                hajjBalance = balance.Hajj,
                maternityBalance = balance.Maternity,
                unpaidBalance = balance.Unpaid,
                compensatoryBalance = balance.Compensatory,
                officialHolidayBalance = balance.OfficialHoliday,
                examBalance = balance.Exam,
                isAutoCalculated = balance.IsAutoCalculated,
                notes = balance.Notes
            });
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpGet]
        [Route("/offdays/balance/notes")]
        public IActionResult GetOffDayBalanceNotes(int employeeId)
        {
            var balance = _context.HROffDayBalances
                .FirstOrDefault(x => x.EmployeeId == employeeId);
            if (balance != null)
            {
                return Json(new { notes = balance.Notes });
            }
            return Json(new { notes = "" });
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpPost]
        [Route("/offdays/balance/edit")]
        public IActionResult EditOffDayBalance([FromBody] BalanceEditRequest request)
        {
            int employeeId = request.EmployeeId;
            Console.WriteLine("Editing off day balance for employeeId: " + employeeId);
            var balance = _context.HROffDayBalances
                .FirstOrDefault(x => x.EmployeeId == employeeId);
            if (balance != null)
            {
                balance.Annual = request.Annual;
                balance.Casual = request.Casual;
                balance.Sick = request.Sick;
                balance.Hajj = request.Hajj;
                balance.Maternity = request.Maternity;
                balance.Unpaid = request.Unpaid;
                balance.Compensatory = request.Compensatory;
                balance.OfficialHoliday = request.OfficialHoliday;
                balance.Exam = request.Exam;
                balance.IsAutoCalculated = false; // manual edit override
                balance.LastUpdated = DateTime.UtcNow;
                _context.HROffDayBalances.Update(balance);
            }
            else
            {
                balance = new HROffDayBalance
                {
                    EmployeeId = employeeId,
                    Annual = request.Annual,
                    Casual = request.Casual,
                    Sick = request.Sick,
                    Hajj = request.Hajj,
                    Maternity = request.Maternity,
                    Unpaid = request.Unpaid,
                    Compensatory = request.Compensatory,
                    OfficialHoliday = request.OfficialHoliday,
                    Exam = request.Exam,
                    Notes = "",
                    IsAutoCalculated = false,
                    LastUpdated = DateTime.UtcNow
                };
                _context.HROffDayBalances.Add(balance);
            }
            _context.SaveChanges();
            return Json(new
            {
                annualBalance = balance.Annual,
                casualBalance = balance.Casual,
                sickBalance = balance.Sick,
                hajjBalance = balance.Hajj,
                maternityBalance = balance.Maternity,
                unpaidBalance = balance.Unpaid,
                compensatoryBalance = balance.Compensatory,
                officialHolidayBalance = balance.OfficialHoliday,
                examBalance = balance.Exam
            });
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpPost]
        [Route("/offdays/balance/notes/edit")]
        public IActionResult EditOffDayBalanceNotes(int employeeId, [FromBody] OffNotesRequest request)
        {
            var balance = _context.HROffDayBalances
                .FirstOrDefault(x => x.EmployeeId == employeeId);
            if (balance != null)
            {
                balance.Notes = request.Notes;
                _context.HROffDayBalances.Update(balance);
            }
            else
            {
                balance = new HROffDayBalance
                {
                    EmployeeId = employeeId,
                    Annual = 0,
                    Casual = 0,
                    Sick = 0,
                    Hajj = 0,
                    Maternity = 0,
                    Unpaid = 0,
                    Compensatory = 0,
                    OfficialHoliday = 0,
                    Exam = 0,
                    Notes = request.Notes,
                    IsAutoCalculated = false,
                    LastUpdated = DateTime.UtcNow
                };
                _context.HROffDayBalances.Add(balance);
            }
            _context.SaveChanges();
            return Ok("Done");
        }

    }
}