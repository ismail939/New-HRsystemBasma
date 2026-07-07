using HRsystem.Models.Enums;

namespace HRsystem.Services
{
    public interface INotificationService
    {
        // Employee notifications
        Task NotifyEmployeeLeaveApproved(int employeeId, int requestId);
        Task NotifyEmployeeLeaveRejected(int employeeId, int requestId, string reason);
        Task NotifyEmployeeLeaveSubmitted(int employeeId, int requestId);

        // HR notifications
        Task NotifyHRNewLeaveRequest(int requestId, string employeeName, LeaveType leaveType);
        Task NotifyHREmployeeLate(int employeeId, DateTime date);
        Task NotifyHREmployeeAbsent(int employeeId, DateTime date);
        Task NotifyHRCheckInOutReview(int employeeId, DateTime date);

        // Admin notifications
        Task NotifyAdminEmployeeRemovalRequest(int adminUserId, string hrName, string employeeName);
        Task NotifyAdminDepartmentCreationRequest(int adminUserId, string hrName, string departmentName);

        // General
        Task<int> GetUnreadCount(int userId);
        Task<List<Models.Notification>> GetRecentNotifications(int userId, int count = 6);
        Task MarkAsRead(int notificationId);
        Task MarkAllAsRead(int userId);
    }
}