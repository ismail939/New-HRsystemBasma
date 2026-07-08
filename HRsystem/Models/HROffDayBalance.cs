using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace HRsystem.Models;

public class HROffDayBalance
{
    [Key]
    public int Id { get; set; }
    
    /// <summary>إجازة سنوية</summary>
    [Required]
    public int Annual { get; set; }
    
    /// <summary>إجازة عارضة (مناسبات)</summary>
    [Required]
    public int Casual { get; set; }
    
    /// <summary>إجازة مرضية</summary>
    [Required]
    public int Sick { get; set; }
    
    /// <summary>إجازة حج</summary>
    [Required]
    public int Hajj { get; set; }
    
    /// <summary>إجازة أمومة</summary>
    [Required]
    public int Maternity { get; set; }
    
    /// <summary>إجازة بدون راتب</summary>
    [Required]
    public int Unpaid { get; set; }
    
    /// <summary>إجازة تعويضي (بدل عطلة وطنية)</summary>
    [Required]
    public int Compensatory { get; set; }
    
    /// <summary>إجازة رسمية (عيد/وطني)</summary>
    [Required]
    public int OfficialHoliday { get; set; }
    
    /// <summary>إجازة اختبارات</summary>
    [Required]
    public int Exam { get; set; }
    
    /// <summary>ملاحظات</summary>
    [Required]
    public string Notes { get; set; } = string.Empty;
    
    /// <summary>تاريخ آخر تحديث للرصيد</summary>
    public DateTime? LastUpdated { get; set; }
    
    /// <summary>هل تم حساب الرصيد تلقائياً</summary>
    public bool IsAutoCalculated { get; set; } = false;
    
    [ForeignKey(nameof(HREmployee))]
    public int? EmployeeId { get; set; }
    [JsonIgnore]
    public virtual HREmployee HREmployee { get; set; }
}