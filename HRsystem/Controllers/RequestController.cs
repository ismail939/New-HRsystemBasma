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
    [Authorize(Roles = "HR,Admin")]
    public class RequestController : Controller
    {
        private readonly AppDbContext _context;
        private readonly INotificationService _notificationService;
        private readonly IOffDayBalanceAutoService _balanceService;

        public RequestController(
            AppDbContext context, 
            INotificationService notificationService,
            IOffDayBalanceAutoService balanceService)
        {
            _context = context;
            _notificationService = notificationService;
            _balanceService = balanceService;
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

            // Check if we have valid dates
            if (request.StartDate == null || request.EndDate == null)
            {
                TempData["Error"] = "طلب الإجازة لا يحتوي على تواريخ صالحة";
                return RedirectToAction("Pending");
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Calculate the number of days
            var daysCount = (int)(request.EndDate.Value - request.StartDate.Value).TotalDays + 1;

            // Try to deduct balance first
            var deducted = await _balanceService.DeductBalanceAsync(
                request.EmployeeId, 
                request.LeaveType, 
                daysCount);

            if (!deducted)
            {
                TempData["Error"] = "الرصيد غير كافٍ لهذا النوع من الإجازات";
                return RedirectToAction("Pending");
            }

            // Create HREmployeeOffDay records for each day
            var currentDate = request.StartDate.Value;
            while (currentDate <= request.EndDate.Value)
            {
                // Check if this day already has an offday record
                var existingOffDay = await _context.HREmployeeOffDays
                    .FirstOrDefaultAsync(o => o.EmployeeId == request.EmployeeId 
                                           && o.OffDayDate.Date == currentDate.Date);

                if (existingOffDay == null)
                {
                    _context.HREmployeeOffDays.Add(new HREmployeeOffDay
                    {
                        EmployeeId = request.EmployeeId,
                        OffDayDate = currentDate,
                        OffDayType = GetLeaveTypeArabicName(request.LeaveType)
                    });
                }

                currentDate = currentDate.AddDays(1);
            }

            request.Status = RequestStatus.Approved;
            request.RespondedAt = DateTime.UtcNow;
            request.RespondedByUserId = userId;
            request.ResponseMessage = model.ResponseMessage;

            await _context.SaveChangesAsync();

            // Notify employee
            await _notificationService.NotifyEmployeeLeaveApproved(request.EmployeeId, request.RequestId);

            TempData["Success"] = "تمت الموافقة على الطلب وخصم الرصيد بنجاح";
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

        /// <summary>
        /// تحويل نوع الإجازة إلى النص العربي للتخزين في HREmployeeOffDay.OffDayType
        /// </summary>
        private string GetLeaveTypeArabicName(LeaveType leaveType)
        {
            return leaveType switch
            {
                LeaveType.Annual => "سنوي",
                LeaveType.Casual => "عارضة",
                LeaveType.Sick => "مرضي",
                LeaveType.Hajj => "حج",
                LeaveType.Maternity => "أمومة",
                LeaveType.Unpaid => "بدون راتب",
                LeaveType.Compensatory => "تعويضي",
                LeaveType.OfficialHoliday => "رسمية",
                LeaveType.Exam => "اختبارات",
                _ => "أخرى"
            };
        }
    }
}