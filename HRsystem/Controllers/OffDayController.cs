using System.Diagnostics;
using HRsystem.Data;
using HRsystem.Models;
using HRsystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRsystem.Controllers
{
    public class OffDayController : Controller
    {
        private readonly ILogger<OffDayController> _logger;
        private readonly AppDbContext _context;
        public OffDayController(ILogger<OffDayController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
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
                    DateOfBirth = (DateTime)emp.DateOfBirth,
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
        public IActionResult Edit([FromBody] EditOffDaysRequest request)
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
                }
                else
                {
                    // Remove from DB if exists
                    var existing = _context.HREmployeeOffDays
                        .FirstOrDefault(x => x.EmployeeId == request.EmployeeId
                                          && x.OffDayDate.Date == date.Date);

                    if (existing != null)
                        _context.HREmployeeOffDays.Remove(existing);
                }
            }
            _context.HRLogs.Add(new HRLog
            {
                Action = $"User ({User.Identity.Name}) edited off days for employeeId ({request.EmployeeId}) for {request.Days.Count} days"
            });
            _context.SaveChanges();

            return Ok("Done");
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpGet]
        [Route("/offdays/balance/add")]
        public IActionResult AddOffDayBalance(int employeeId)
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
                offBalance = existing.Off,
                insteadBalance = existing.CompensatoryOfNationalHoliday
            });
            }
            var balance = new HROffDayBalance
            {
                EmployeeId = employeeId,
                Annual = 0,
                Casual = 0,
                Off = 0,
                CompensatoryOfNationalHoliday = 0,
                Notes = ""
            };
            _context.HROffDayBalances.Add(balance);
            _context.SaveChanges();
            return Json(new
            {
                annualBalance = balance.Annual,
                casualBalance = balance.Casual,
                offBalance = balance.Off,
                insteadBalance = balance.CompensatoryOfNationalHoliday
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
            int annual = request.Annual;
            int casual = request.Casual;
            int off = request.Off;
            int instead = request.Instead;
            Console.WriteLine("Editing off day balance for employeeId: " + employeeId);
            var balance = _context.HROffDayBalances
                .FirstOrDefault(x => x.EmployeeId == employeeId);
            if (balance != null)
            {
                balance.Annual = annual;
                balance.Casual = casual;
                balance.Off = off;
                balance.CompensatoryOfNationalHoliday = instead;
                _context.HROffDayBalances.Update(balance);
            }
            else
            {
                balance = new HROffDayBalance
                {
                    EmployeeId = employeeId,
                    Annual = annual,
                    Casual = casual,
                    Off = off,
                    CompensatoryOfNationalHoliday = instead,
                    Notes = ""
                };
                _context.HROffDayBalances.Add(balance);
            }
            _context.SaveChanges();
            return Json(new
            {
                annualBalance = balance.Annual,
                casualBalance = balance.Casual,
                offBalance = balance.Off,
                insteadBalance = balance.CompensatoryOfNationalHoliday
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
                    Off = 0,
                    CompensatoryOfNationalHoliday = 0,
                    Notes = request.Notes
                };
                _context.HROffDayBalances.Add(balance);
            }
            _context.SaveChanges();
            return Ok("Done");
        }

    }
}