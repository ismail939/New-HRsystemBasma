using System.Diagnostics;
using HRsystem.Data;
using HRsystem.Models;
using HRsystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRsystem.Controllers
{
    public class PenaltyController : Controller
    {
        private readonly ILogger<PenaltyController> _logger;
        private readonly AppDbContext _context;
        public PenaltyController(ILogger<PenaltyController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpGet]
        [Route("/penalties")]
        public IActionResult Penalties()
        {
            Console.WriteLine("⭐Entered ListEmployees action");
            var employees = _context.HREmployees.ToList();
            Console.WriteLine("here is the number of the items: " + employees.Count);
            foreach (var emp in employees)
            {
                Console.WriteLine(emp.Name + " - " + emp.HRDepartmentId);
            }
            var employeeVMs = new List<SimpleEmployeeViewModel>();
            foreach (var emp in employees)
            {
                Console.WriteLine($"Processing employee: {emp.Name} with department ID: {emp.HRDepartmentId}");
                var dep = _context.HRDepartments.FirstOrDefault(d => d.Id == emp.HRDepartmentId);
                string depName = dep != null ? dep.Name : "";
                employeeVMs.Add(new SimpleEmployeeViewModel
                {
                    Id = emp.Id,
                    Name = emp.Name,
                    PhoneNumber = emp.PhoneNumber,
                    HireDate = emp.HireDate,
                    JobName = emp.JobName,
                    Department = depName==""?"": depName
                });
            }

            var penaltyList = _context.HREmployeePenalties.ToList();
            Console.WriteLine("penaltyList.Count: " + penaltyList.Count);
            foreach (var penalty in penaltyList)
            {
                Console.WriteLine($"🟢 Penalty ID: {penalty.Id}, Employee ID: {penalty.EmployeeId}, Decision: {penalty.Decision}, Date: {penalty.PenaltyDate}, Reason: {penalty.Reason}");
            }
        
            var list = new EmployeesNPenalties
            {
                Employees = employeeVMs,
                Penalties = penaltyList
            };

            return View(list);
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpPost]
        [Route("/employee/addPenalty")]
        public IActionResult AddPenalty(string penalty, DateTime dayDate, string reason, int employeeId, int PenaltyPoints)
        {
            Console.WriteLine($"🔵Received data - Penalty: {penalty}, Date: {dayDate}, Reason: {reason}, EmployeeId: {employeeId}");
            if (string.IsNullOrEmpty(penalty) || string.IsNullOrEmpty(reason) || employeeId <= 0|| dayDate == default)
            {
                Console.WriteLine("🔴Invalid input data.");
                return BadRequest("Invalid input data.");
            }
            var newPenalty = new HREmployeePenalty
            {
                Decision = penalty,
                PenaltyDate = dayDate,
                PenaltyPoints = PenaltyPoints,
                Reason = reason,
                EmployeeId = employeeId
            };
            _context.HREmployeePenalties.Add(newPenalty);
            _context.HRLogs.Add(new HRLog
            {
                Action = $"User ({User.Identity.Name}) added penalty ({penalty}) for employeeId ({employeeId}) with reason ({reason}) and points ({PenaltyPoints})"
            });
            _context.SaveChanges();
            return Ok("Penalty added successfully.");
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpGet]
        [Route("/employee/penalties/{employeeId}")]
        public IActionResult GetEmployeePenalties(int employeeId)
        {
            var penalties = _context.HREmployeePenalties
                .Where(p => p.EmployeeId == employeeId)
                .ToList();
            return Json(penalties);
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpPost]
        [Route("/employee/togglePenaltyActive")]
        public IActionResult TogglePenaltyActive([FromBody] PenaltyToggleRequest RequestBody)
        {
            Console.WriteLine(RequestBody+"🔵 Received toggle request.");
            bool isActive = RequestBody.IsActive;
            int penaltyId = RequestBody.PenaltyId;
            Console.WriteLine($"🔵 Toggling penalty ID {penaltyId} to active status: {isActive}");
            var penalty = _context.HREmployeePenalties.Find(penaltyId);
            if (penalty == null)
            {
                return NotFound("Penalty not found.");
            }
            penalty.IsActive = isActive;
            _context.HRLogs.Add(new HRLog
            {
                Action = $"User ({User.Identity.Name}) toggled penalty ID ({penaltyId}) active status to ({isActive}) for employeeId ({penalty.EmployeeId})"
            });
            _context.SaveChanges();
            return Ok("Penalty active status updated.");
        }

    }
}