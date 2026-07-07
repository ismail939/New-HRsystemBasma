using System.Security.Claims;
using HRsystem.Data;
using HRsystem.Models.Enums;
using HRsystem.Services;
using HRsystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRsystem.Controllers
{
    [Authorize(Roles = "HR,Admin")]
    public class RequestController : Controller
    {
        private readonly AppDbContext _context;
        private readonly INotificationService _notificationService;

        public RequestController(AppDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        [HttpGet]
        [Route("/requests/pending")]
        public async Task<IActionResult> Pending()
        {
            var requests = await _context.Requests
                .Where(r => r.Status == RequestStatus.Pending && r.RequestType == RequestType.Leave)
                .Include(r => r.Employee)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ManageRequestViewModel
                {
                    RequestId = r.RequestId,
                    EmployeeName = r.Employee.Name,
                    EmployeeNationalId = r.Employee.NationalId ?? "",
                    RequestType = r.RequestType,
                    LeaveType = r.LeaveType,
                    Status = r.Status,
                    Reason = r.Reason ?? "",
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            return View(requests);
        }

        [HttpPost]
        [Route("/requests/approve/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id, RespondToRequestViewModel model)
        {
            var request = await _context.Requests
                .Include(r => r.Employee)
                .FirstOrDefaultAsync(r => r.RequestId == id);

            if (request == null)
                return NotFound();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            request.Status = RequestStatus.Approved;
            request.RespondedAt = DateTime.UtcNow;
            request.RespondedByUserId = userId;
            request.ResponseMessage = model.ResponseMessage;

            await _context.SaveChangesAsync();

            // Notify employee
            await _notificationService.NotifyEmployeeLeaveApproved(request.EmployeeId, request.RequestId);

            TempData["Success"] = "تمت الموافقة على الطلب بنجاح";
            return RedirectToAction("Pending");
        }

        [HttpPost]
        [Route("/requests/reject/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, RespondToRequestViewModel model)
        {
            var request = await _context.Requests
                .Include(r => r.Employee)
                .FirstOrDefaultAsync(r => r.RequestId == id);

            if (request == null)
                return NotFound();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            request.Status = RequestStatus.Rejected;
            request.RespondedAt = DateTime.UtcNow;
            request.RespondedByUserId = userId;
            request.ResponseMessage = model.ResponseMessage;

            await _context.SaveChangesAsync();

            // Notify employee
            await _notificationService.NotifyEmployeeLeaveRejected(
                request.EmployeeId,
                request.RequestId,
                model.ResponseMessage ?? "لم يتم تقديم سبب");

            TempData["Success"] = "تم رفض الطلب";
            return RedirectToAction("Pending");
        }

        [HttpGet]
        [Route("/requests/history")]
        public async Task<IActionResult> History()
        {
            var requests = await _context.Requests
                .Where(r => r.RequestType == RequestType.Leave)
                .Include(r => r.Employee)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ManageRequestViewModel
                {
                    RequestId = r.RequestId,
                    EmployeeName = r.Employee.Name,
                    EmployeeNationalId = r.Employee.NationalId ?? "",
                    RequestType = r.RequestType,
                    LeaveType = r.LeaveType,
                    Status = r.Status,
                    Reason = r.Reason ?? "",
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            return View(requests);
        }
    }
}