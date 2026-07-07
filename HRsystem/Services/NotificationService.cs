using HRsystem.Data;
using HRsystem.Models;
using HRsystem.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace HRsystem.Services
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _context;

        public NotificationService(AppDbContext context)
        {
            _context = context;
        }

        // ===== Employee Notifications =====

        public async Task NotifyEmployeeLeaveApproved(int employeeId, int requestId)
        {
            var user = await GetUserByEmployeeId(employeeId);
            if (user == null) return;

            var request = await _context.Requests.FindAsync(requestId);
            if (request == null) return;

            var leaveTypeName = GetLeaveTypeArabicName(request.LeaveType);

            await CreateNotification(new Notification
            {
                UserId = user.Id,
                Title = "تم الموافقة على طلب الإجازة",
                Message = $"تمت الموافقة على طلب إجازة {leaveTypeName} الخاص بك.",
                NotificationType = NotificationType.LeaveApproved,
                RelatedRequestId = requestId
            });
        }

        public async Task NotifyEmployeeLeaveRejected(int employeeId, int requestId, string reason)
        {
            var user = await GetUserByEmployeeId(employeeId);
            if (user == null) return;

            var request = await _context.Requests.FindAsync(requestId);
            if (request == null) return;

            var leaveTypeName = GetLeaveTypeArabicName(request.LeaveType);

            await CreateNotification(new Notification
            {
                UserId = user.Id,
                Title = "تم رفض طلب الإجازة",
                Message = $"تم رفض طلب إجازة {leaveTypeName}. السبب: {reason}",
                NotificationType = NotificationType.LeaveRejected,
                RelatedRequestId = requestId
            });
        }

        // Employee notification when submitting - simplified, and now includes dates
        public async Task NotifyEmployeeLeaveSubmitted(int employeeId, int requestId)
        {
            var user = await GetUserByEmployeeId(employeeId);
            if (user == null) return;

            var request = await _context.Requests.FindAsync(requestId);
            if (request == null) return;

            var leaveTypeName = GetLeaveTypeArabicName(request.LeaveType);
            var dateInfo = "";
            if (request.StartDate.HasValue)
            {
                dateInfo = request.EndDate.HasValue && request.EndDate.Value != request.StartDate.Value
                    ? $" من {request.StartDate.Value:yyyy-MM-dd} إلى {request.EndDate.Value:yyyy-MM-dd}"
                    : $" في {request.StartDate.Value:yyyy-MM-dd}";
            }

            await CreateNotification(new Notification
            {
                UserId = user.Id,
                Title = "قيد الانتظار",
                Message = $"طلب إجازة {leaveTypeName}{dateInfo} قيد انتظار الموافقة.",
                NotificationType = NotificationType.LeaveRequest,
                RelatedRequestId = requestId
            });
        }

        // ===== HR Notifications =====

        public async Task NotifyHRNewLeaveRequest(int requestId, string employeeName, LeaveType leaveType)
        {
            var hrUsers = await _context.Users.Where(u => u.Role == "HR" && u.IsActive).ToListAsync();
            var leaveTypeName = GetLeaveTypeArabicName(leaveType);

            foreach (var hr in hrUsers)
            {
                await CreateNotification(new Notification
                {
                    UserId = hr.Id,
                    Title = "طلب إجازة جديد",
                    Message = $"قام الموظف {employeeName} بتقديم طلب إجازة {leaveTypeName}.",
                    NotificationType = NotificationType.LeaveRequest,
                    RelatedRequestId = requestId
                });
            }
        }

        public async Task NotifyHREmployeeLate(int employeeId, DateTime date)
        {
            var hrUsers = await _context.Users.Where(u => u.Role == "HR" && u.IsActive).ToListAsync();
            var employee = await _context.HREmployees.FindAsync(employeeId);
            var employeeName = employee?.Name ?? "موظف";

            foreach (var hr in hrUsers)
            {
                await CreateNotification(new Notification
                {
                    UserId = hr.Id,
                    Title = "موظف متأخر",
                    Message = $"الموظف {employeeName} متأخر في يوم {date:yyyy-MM-dd}.",
                    NotificationType = NotificationType.EmployeeLate
                });
            }
        }

        public async Task NotifyHREmployeeAbsent(int employeeId, DateTime date)
        {
            var hrUsers = await _context.Users.Where(u => u.Role == "HR" && u.IsActive).ToListAsync();
            var employee = await _context.HREmployees.FindAsync(employeeId);
            var employeeName = employee?.Name ?? "موظف";

            foreach (var hr in hrUsers)
            {
                await CreateNotification(new Notification
                {
                    UserId = hr.Id,
                    Title = "موظف غائب",
                    Message = $"الموظف {employeeName} لم يسجل حضور أو انصراف في يوم {date:yyyy-MM-dd}.",
                    NotificationType = NotificationType.EmployeeAbsent
                });
            }
        }

        public async Task NotifyHRCheckInOutReview(int employeeId, DateTime date)
        {
            var hrUsers = await _context.Users.Where(u => u.Role == "HR" && u.IsActive).ToListAsync();
            var employee = await _context.HREmployees.FindAsync(employeeId);
            var employeeName = employee?.Name ?? "موظف";

            foreach (var hr in hrUsers)
            {
                await CreateNotification(new Notification
                {
                    UserId = hr.Id,
                    Title = "حضور يحتاج مراجعة",
                    Message = $"حضور الموظف {employeeName} في يوم {date:yyyy-MM-dd} يحتاج إلى مراجعة.",
                    NotificationType = NotificationType.EmployeeAbsent
                });
            }
        }

        // ===== Admin Notifications =====

        public async Task NotifyAdminEmployeeRemovalRequest(int adminUserId, string hrName, string employeeName)
        {
            await CreateNotification(new Notification
            {
                UserId = adminUserId,
                Title = "طلب حذف موظف",
                Message = $"قام {hrName} بطلب حذف الموظف {employeeName}.",
                NotificationType = NotificationType.EmployeeRemovalRequest
            });
        }

        public async Task NotifyAdminDepartmentCreationRequest(int adminUserId, string hrName, string departmentName)
        {
            await CreateNotification(new Notification
            {
                UserId = adminUserId,
                Title = "طلب إنشاء قسم",
                Message = $"قام {hrName} بطلب إنشاء قسم جديد: {departmentName}.",
                NotificationType = NotificationType.DepartmentCreationRequest
            });
        }

        // ===== General =====

        public async Task<int> GetUnreadCount(int userId)
        {
            return await _context.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task<List<Notification>> GetRecentNotifications(int userId, int count = 6)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task MarkAsRead(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsRead(int userId)
        {
            var unread = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var n in unread)
            {
                n.IsRead = true;
            }

            await _context.SaveChangesAsync();
        }

        // ===== Helpers =====

        private async Task CreateNotification(Notification notification)
        {
            notification.CreatedAt = DateTime.UtcNow;
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        private async Task<User?> GetUserByEmployeeId(int employeeId)
        {
            var employee = await _context.HREmployees.FindAsync(employeeId);
            if (employee == null || string.IsNullOrEmpty(employee.NationalId)) return null;

            return await _context.Users.FirstOrDefaultAsync(u => u.Username == employee.NationalId);
        }

        private static string GetLeaveTypeArabicName(LeaveType leaveType)
        {
            return leaveType switch
            {
                LeaveType.Casual => "عارضة",
                LeaveType.Annual => "سنوية",
                LeaveType.Sick => "مرضية",
                _ => ""
            };
        }
    }
}