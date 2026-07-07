using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRsystem.Models.Enums;

namespace HRsystem.Models
{
    public class Request
    {
        [Key]
        public int RequestId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual HREmployee Employee { get; set; }

        [Required]
        public int CreatedByUserId { get; set; }

        [ForeignKey("CreatedByUserId")]
        public virtual User CreatedByUser { get; set; }

        [Required]
        public RequestType RequestType { get; set; }

        public LeaveType LeaveType { get; set; } = Enums.LeaveType.None;

        [Required]
        public RequestStatus Status { get; set; } = RequestStatus.Pending;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [MaxLength(1000)]
        public string? Reason { get; set; }

        [MaxLength(1000)]
        public string? ResponseMessage { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? RespondedAt { get; set; }

        public int? RespondedByUserId { get; set; }

        [ForeignKey("RespondedByUserId")]
        public virtual User? RespondedByUser { get; set; }

        // Navigation
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}