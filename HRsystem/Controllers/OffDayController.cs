using System.Diagnostics;
using HRsystem.Data;
using HRsystem.Models;
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
        [HttpGet]
        [Route("/OffDays")]
        public IActionResult OffDay()
        {
            var list = _context.HREmployees.ToList();
            return View(list);
        }
        [HttpGet("employees/offdays/{employeeId}")]
        public IActionResult GetOffDays(
            int employeeId,
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            Console.WriteLine(string.Format("employeeId: {0}, from: {1}, to: {2}", employeeId, from, to));
            var list = _context.HREmployeeOffDays.Where(x=> x.EmployeeId == employeeId &&
            x.OffDayDate >= from &&
            x.OffDayDate <= to
            ).OrderBy(x=> x.OffDayDate).ToList();
            return Json(list);
        }
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
                    if (existing!=null)
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

            _context.SaveChanges();

            return Ok("Done");
        }


    }
}