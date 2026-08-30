using System.Diagnostics;
using HRsystem.Data;
using HRsystem.Models;
using HRsystem.Models.Enums;
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
            var employees = _context.HREmployees.ToList();
            var employeeVMs = new List<SimpleEmployeeViewModel>();
            foreach (var emp in employees)
            {
                var dep = _context.HRDepartments.FirstOrDefault(d => d.Id == emp.HRDepartmentId);
                string depName = dep != null ? dep.Name : "";
                employeeVMs.Add(new SimpleEmployeeViewModel
                {
                    Id = emp.Id,
                    Name = emp.Name,
                    PhoneNumber = emp.PhoneNumber,
                    HireDate = emp.HireDate,
                    JobName = emp.JobName,
                    Department = depName == "" ? "" : depName
                });
            }

            var list = new EmployeesNPenalties
            {
                Employees = employeeVMs,
                Penalties = new List<object>()
            };

            return View(list);
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpGet]
        [Route("/penalty/rules")]
        public IActionResult GetRules()
        {
            var rules = _context.PenaltyRules
                .Where(r => r.IsActive)
                .OrderBy(r => r.NameAr)
                .Select(r => new { r.Id, r.Name, r.NameAr, Category = (int)r.Category })
                .ToList();
            return Json(rules);
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpPost]
        [Route("/employee/addPenalty")]
        public IActionResult AddPenalty(int employeeId, int ruleId, DateTime incidentDate, int deductionUnit, decimal deductionValue, string? notes)
        {
            if (employeeId <= 0 || ruleId <= 0 || incidentDate == default)
            {
                return BadRequest("Invalid input data.");
            }
            var rule = _context.PenaltyRules.Find(ruleId);
            if (rule == null)
            {
                return BadRequest("القاعدة غير موجودة.");
            }

            var newPenalty = new EmployeePenalty
            {
                EmployeeId = employeeId,
                PenaltyRuleId = ruleId,
                IncidentDate = incidentDate,
                CreatedDate = DateTime.Now,
                Status = PenaltyStatus.Draft,
                DeductionUnit = (DeductionUnit)deductionUnit,
                DeductionValue = deductionValue,
                ManagerNotes = notes
            };
            _context.EmployeePenalties.Add(newPenalty);
            _context.HRLogs.Add(new HRLog
            {
                Action = $"User ({User.Identity.Name}) added penalty rule ({rule.Name}) for employeeId ({employeeId}) on ({incidentDate:yyyy-MM-dd})"
            });
            _context.SaveChanges();
            return Ok("Penalty added successfully.");
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpGet]
        [Route("/employee/penalties/{employeeId}")]
        public IActionResult GetEmployeePenalties(int employeeId)
        {
            var penalties = _context.EmployeePenalties
                .Where(p => p.EmployeeId == employeeId)
                .OrderByDescending(p => p.IncidentDate)
                .Select(p => new
                {
                    p.Id,
                    p.EmployeeId,
                    p.PenaltyRuleId,
                    p.PenaltyLevelId,
                    p.IncidentDate,
                    p.CreatedDate,
                    Status = (int)p.Status,
                    DeductionUnit = (int)p.DeductionUnit,
                    p.DeductionValue,
                    p.DeductionDays,
                    p.DeductionAmount,
                    p.PayrollItemId,
                    p.ManagerNotes,
                    p.ApprovedDate,
                    RuleNameAr = p.PenaltyRule.NameAr,
                    RuleName = p.PenaltyRule.Name,
                    IsActive = p.Status == PenaltyStatus.Approved
                })
                .ToList();
            return Json(penalties);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        [Route("/employee/togglePenaltyActive")]
        public IActionResult TogglePenaltyActive([FromBody] PenaltyToggleRequest RequestBody)
        {
            bool isActive = RequestBody.IsActive;
            int penaltyId = RequestBody.PenaltyId;
            var penalty = _context.EmployeePenalties.Find(penaltyId);
            if (penalty == null)
            {
                return NotFound("Penalty not found.");
            }

            if (isActive)
            {
                // Make eligible for payroll (Approved)
                if (penalty.Status == PenaltyStatus.Draft || penalty.Status == PenaltyStatus.PendingApproval)
                {
                    penalty.Status = PenaltyStatus.Approved;
                    penalty.ApprovedDate = DateTime.Now;
                    var userName = User.Identity?.Name;
                    var user = userName != null ? _context.Users.FirstOrDefault(u => u.Username == userName) : null;
                    penalty.ApprovedByUserId = user?.Id;
                }
            }
            else
            {
                // Deactivate -> back to Draft (no payroll impact)
                if (penalty.Status == PenaltyStatus.Approved)
                {
                    penalty.Status = PenaltyStatus.Draft;
                    penalty.ApprovedDate = null;
                    penalty.ApprovedByUserId = null;
                }
            }

            _context.HRLogs.Add(new HRLog
            {
                Action = $"User ({User.Identity.Name}) set penalty ID ({penaltyId}) active status to ({isActive}) for employeeId ({penalty.EmployeeId})"
            });
            _context.SaveChanges();
            return Ok("Penalty active status updated.");
        }
    }
}
