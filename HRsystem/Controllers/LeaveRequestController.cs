using System.Security.Claims;
using HRsystem.Data;
using HRsystem.Models;
using HRsystem.Models.Enums;
using HRsystem.Services;
using HRsystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRsystem.Controllers
{
    [Authorize(Roles = "Employee")]
    public class LeaveRequestController : Controller
    {
        private readonly AppDbContext _context;
        private readonly INotificationService _notificationService;

        public LeaveRequestController(AppDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        [HttpGet]
        [Route("/employee-dashboard/request-vacation")]
        public IActionResult Create()
        {
            var model = new CreateLeaveRequestViewModel
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(1)
            };
            return View(model);
        }

        [HttpPost]
        [Route("/employee-dashboard/request-vacation")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateLeaveRequestViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.EndDate < model.StartDate)
            {
                ModelState.AddModelError("EndDate", "تاريخ النهاية يجب أن يكون بعد تاريخ البداية");
                return View(model);
            }

            string nationalId = User.FindFirstValue("NationalId") ?? "";
            var employee = await _context.HREmployees.FirstOrDefaultAsync(e => e.NationalId == nationalId);
            if (employee == null)
            {
                ModelState.AddModelError("", "الموظف غير موجود");
                return View(model);
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                ModelState.AddModelError("", "مستخدم غير معروف");
                return View(model);
            }
            var userId = int.Parse(userIdClaim);

            var request = new Request
            {
                EmployeeId = employee.Id,
                CreatedByUserId = userId,
                RequestType = RequestType.Leave,
                LeaveType = model.LeaveType,
                Status = RequestStatus.Pending,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Reason = model.Reason,
                CreatedAt = DateTime.UtcNow
            };

            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            // Notify employee that request was submitted
            await _notificationService.NotifyEmployeeLeaveSubmitted(employee.Id, request.RequestId);

            // Notify HR about new leave request
            await _notificationService.NotifyHRNewLeaveRequest(request.RequestId, employee.Name, model.LeaveType);

            TempData["Success"] = "تم تقديم طلب الإجازة بنجاح";
            return RedirectToAction("MyRequests");
        }

        [HttpGet]
        [Route("/employee-dashboard/my-requests")]
        public async Task<IActionResult> MyRequests()
        {
            string nationalId = User.FindFirstValue("NationalId") ?? "";
            var employee = await _context.HREmployees.FirstOrDefaultAsync(e => e.NationalId == nationalId);
            if (employee == null)
                return View(new List<LeaveRequestListViewModel>());

            var requests = await _context.Requests
                .Where(r => r.EmployeeId == employee.Id && r.RequestType == RequestType.Leave)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new LeaveRequestListViewModel
                {
                    RequestId = r.RequestId,
                    LeaveType = r.LeaveType,
                    Status = r.Status,
                    Reason = r.Reason ?? "",
                    ResponseMessage = r.ResponseMessage,
                    CreatedAt = r.CreatedAt,
                    RespondedAt = r.RespondedAt,
                    RespondedByName = r.RespondedByUser != null ? r.RespondedByUser.Username : null
                })
                .ToListAsync();

            return View(requests);
        }
    }
}