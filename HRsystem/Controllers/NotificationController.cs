using System.Security.Claims;
using HRsystem.Data;
using HRsystem.Services;
using HRsystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRsystem.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly AppDbContext _context;

        public NotificationController(INotificationService notificationService, AppDbContext context)
        {
            _notificationService = notificationService;
            _context = context;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [HttpGet]
        [Route("/notifications/unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = GetUserId();
            var count = await _notificationService.GetUnreadCount(userId);
            return Json(new { count });
        }

        [HttpGet]
        [Route("/notifications/recent")]
        public async Task<IActionResult> GetRecent()
        {
            var userId = GetUserId();
            var notifications = await _notificationService.GetRecentNotifications(userId, 6);
            var unreadCount = await _notificationService.GetUnreadCount(userId);

            var viewModels = notifications.Select(n => new NotificationViewModel
            {
                NotificationId = n.NotificationId,
                Title = n.Title,
                Message = n.Message,
                NotificationType = n.NotificationType,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt,
                RelativeTime = GetRelativeTime(n.CreatedAt),
                RelatedRequestId = n.RelatedRequestId,
                NavigateUrl = GetNavigateUrl(n.NotificationType, n.RelatedRequestId)
            }).ToList();

            return Json(new NotificationListViewModel
            {
                Notifications = viewModels,
                UnreadCount = unreadCount,
                TotalCount = notifications.Count
            });
        }

        [HttpPost]
        [Route("/notifications/mark-read/{id}")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _notificationService.MarkAsRead(id);
            return Json(new { success = true });
        }

        [HttpPost]
        [Route("/notifications/mark-all-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = GetUserId();
            await _notificationService.MarkAllAsRead(userId);
            return Json(new { success = true });
        }

        [HttpGet]
        [Route("/Notifications")]
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            var notifications = await _notificationService.GetRecentNotifications(userId, 50);
            var unreadCount = await _notificationService.GetUnreadCount(userId);

            var viewModels = notifications.Select(n => new NotificationViewModel
            {
                NotificationId = n.NotificationId,
                Title = n.Title,
                Message = n.Message,
                NotificationType = n.NotificationType,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt,
                RelativeTime = GetRelativeTime(n.CreatedAt),
                RelatedRequestId = n.RelatedRequestId,
                NavigateUrl = GetNavigateUrl(n.NotificationType, n.RelatedRequestId)
            }).ToList();

            var model = new NotificationListViewModel
            {
                Notifications = viewModels,
                UnreadCount = unreadCount,
                TotalCount = _context.Notifications.Count(n => n.UserId == userId)
            };

            return View(model);
        }

        private static string GetNavigateUrl(Models.Enums.NotificationType type, int? relatedRequestId)
        {
            return type switch
            {
                Models.Enums.NotificationType.LeaveRequest or
                Models.Enums.NotificationType.LeaveApproved or
                Models.Enums.NotificationType.LeaveRejected => "/requests/pending",
                Models.Enums.NotificationType.EmployeeLate or
                Models.Enums.NotificationType.EmployeeAbsent => "/basma",
                Models.Enums.NotificationType.EmployeeRemovalRequest or
                Models.Enums.NotificationType.DepartmentCreationRequest => "/admin/dashboard",
                _ => "/Notifications"
            };
        }

        private static string GetRelativeTime(DateTime utcDateTime)
        {
            // Convert UTC to Egypt local time (UTC+2 or UTC+3 during DST)
            var egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, egyptTimeZone);
            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, egyptTimeZone);
            var diff = now - localTime;

            if (diff.TotalMinutes < 1) return "الآن";
            if (diff.TotalMinutes < 60) return $"منذ {Math.Floor(diff.TotalMinutes)} دقيقة";
            if (diff.TotalHours < 24) return $"منذ {Math.Floor(diff.TotalHours)} ساعة";
            if (diff.TotalDays < 7) return $"منذ {Math.Floor(diff.TotalDays)} يوم";
            if (diff.TotalDays < 30) return $"منذ {Math.Floor(diff.TotalDays / 7)} أسبوع";
            if (diff.TotalDays < 365) return $"منذ {Math.Floor(diff.TotalDays / 30)} شهر";
            return $"منذ {Math.Floor(diff.TotalDays / 365)} سنة";
        }
    }
}