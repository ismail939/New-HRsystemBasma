using System.Diagnostics;
using HRsystem.Data;
using HRsystem.Models;
using HRsystem.ViewModels;
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
        [HttpGet]
        [Route("/penalties")]
        public IActionResult Penalties()
        {
            var empList = _context.HREmployees.ToList();
            var penaltyList = _context.HREmployeePenalties.ToList();
            Console.WriteLine("penaltyList.Count: " + penaltyList.Count);
            foreach (var penalty in penaltyList)
            {
                Console.WriteLine($"🟢 Penalty ID: {penalty.Id}, Employee ID: {penalty.EmployeeId}, Decision: {penalty.Decision}, Date: {penalty.PenaltyDate}, Reason: {penalty.Reason}");
            }
            var list = new EmployeesNPenalties
            {
                Employees = empList,
                Penalties = penaltyList
            };

            return View(list);
        }
        [HttpPost]
        [Route("/employee/addPenalty")]
        public IActionResult AddPenalty(string penalty, DateTime dayDate, string reason, int employeeId)
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
                Reason = reason,
                EmployeeId = employeeId
            };
            _context.HREmployeePenalties.Add(newPenalty);
            _context.SaveChanges();
            return Ok("Penalty added successfully.");
        }
        [HttpGet]
        [Route("/employee/penalties/{employeeId}")]
        public IActionResult GetEmployeePenalties(int employeeId)
        {
            var penalties = _context.HREmployeePenalties
                .Where(p => p.EmployeeId == employeeId)
                .ToList();
            return Json(penalties);
        }
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
            _context.SaveChanges();
            return Ok("Penalty active status updated.");
        }

    }
}