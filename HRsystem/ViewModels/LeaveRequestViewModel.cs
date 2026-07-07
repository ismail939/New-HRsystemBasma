using HRsystem.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace HRsystem.ViewModels
{
    public class CreateLeaveRequestViewModel
    {
        [Required(ErrorMessage = "يرجى اختيار نوع الإجازة")]
        public LeaveType LeaveType { get; set; }

        [Required(ErrorMessage = "يرجى تحديد تاريخ البداية")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "يرجى تحديد تاريخ النهاية")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; } = DateTime.Today.AddDays(1);

        [MaxLength(1000)]
        public string? Reason { get; set; }
    }

    public class LeaveRequestListViewModel
    {
        public int RequestId { get; set; }
        public LeaveType LeaveType { get; set; }
        public RequestStatus Status { get; set; }
        public string Reason { get; set; }
        public string? ResponseMessage { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? RespondedAt { get; set; }
        public string? RespondedByName { get; set; }

        public string LeaveTypeName => LeaveType switch
        {
            LeaveType.Casual => "عارضة",
            LeaveType.Annual => "سنوية",
            LeaveType.Sick => "مرضية",
            _ => ""
        };

        public string StatusName => Status switch
        {
            RequestStatus.Pending => "قيد الانتظار",
            RequestStatus.Approved => "تمت الموافقة",
            RequestStatus.Rejected => "مرفوض",
            _ => ""
        };

        public string StatusColorClass => Status switch
        {
            RequestStatus.Pending => "bg-yellow-100 text-yellow-800 border-yellow-300",
            RequestStatus.Approved => "bg-green-100 text-green-800 border-green-300",
            RequestStatus.Rejected => "bg-red-100 text-red-800 border-red-300",
            _ => "bg-gray-100 text-gray-800"
        };

        public string IconClass => Status switch
        {
            RequestStatus.Pending => "bi bi-hourglass-split",
            RequestStatus.Approved => "bi bi-check-circle-fill text-green-600",
            RequestStatus.Rejected => "bi bi-x-circle-fill text-red-600",
            _ => "bi bi-question-circle"
        };
    }

    public class ManageRequestViewModel
    {
        public int RequestId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeNationalId { get; set; }
        public RequestType RequestType { get; set; }
        public LeaveType LeaveType { get; set; }
        public RequestStatus Status { get; set; }
        public string Reason { get; set; }
        public DateTime CreatedAt { get; set; }

        public string RequestTypeName => RequestType switch
        {
            RequestType.Leave => "إجازة",
            RequestType.EmployeeRemoval => "حذف موظف",
            RequestType.DepartmentCreation => "إنشاء قسم",
            _ => ""
        };

        public string LeaveTypeName => LeaveType switch
        {
            LeaveType.Casual => "عارضة",
            LeaveType.Annual => "سنوية",
            LeaveType.Sick => "مرضية",
            _ => ""
        };
    }

    public class RespondToRequestViewModel
    {
        public int RequestId { get; set; }
        [MaxLength(1000)]
        public string? ResponseMessage { get; set; }
    }
}