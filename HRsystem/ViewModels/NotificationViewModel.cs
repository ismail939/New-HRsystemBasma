using HRsystem.Models.Enums;

namespace HRsystem.ViewModels
{
    public class NotificationViewModel
    {
        public int NotificationId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public NotificationType NotificationType { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public string RelativeTime { get; set; }
        public int? RelatedRequestId { get; set; }
        public string NavigateUrl { get; set; }

        // Computed display properties
        public string IconClass => GetIconClass();
        public string BorderColorClass => GetBorderColorClass();
        public string BadgeColorClass => GetBadgeColorClass();

        private string GetIconClass()
        {
            return NotificationType switch
            {
                NotificationType.LeaveRequest => "bi bi-calendar-plus",
                NotificationType.LeaveApproved => "bi bi-check-circle",
                NotificationType.LeaveRejected => "bi bi-x-circle",
                NotificationType.EmployeeLate => "bi bi-clock",
                NotificationType.EmployeeAbsent => "bi bi-person-x",
                NotificationType.EmployeeRemovalRequest => "bi bi-person-dash",
                NotificationType.DepartmentCreationRequest => "bi bi-building-add",
                _ => "bi bi-bell"
            };
        }

        private string GetBorderColorClass()
        {
            return NotificationType switch
            {
                NotificationType.LeaveRequest => "border-l-blue-500",
                NotificationType.LeaveApproved => "border-l-green-500",
                NotificationType.LeaveRejected => "border-l-red-500",
                NotificationType.EmployeeLate => "border-l-yellow-500",
                NotificationType.EmployeeAbsent => "border-l-orange-500",
                NotificationType.EmployeeRemovalRequest => "border-l-red-600",
                NotificationType.DepartmentCreationRequest => "border-l-purple-500",
                _ => "border-l-gray-400"
            };
        }

        private string GetBadgeColorClass()
        {
            return NotificationType switch
            {
                NotificationType.LeaveApproved => "bg-green-100 text-green-800",
                NotificationType.LeaveRejected => "bg-red-100 text-red-800",
                NotificationType.LeaveRequest => "bg-blue-100 text-blue-800",
                NotificationType.EmployeeLate => "bg-yellow-100 text-yellow-800",
                NotificationType.EmployeeAbsent => "bg-orange-100 text-orange-800",
                NotificationType.EmployeeRemovalRequest => "bg-red-100 text-red-800",
                NotificationType.DepartmentCreationRequest => "bg-purple-100 text-purple-800",
                _ => "bg-gray-100 text-gray-800"
            };
        }
    }

    public class NotificationListViewModel
    {
        public List<NotificationViewModel> Notifications { get; set; } = new();
        public int UnreadCount { get; set; }
        public int TotalCount { get; set; }
    }
}