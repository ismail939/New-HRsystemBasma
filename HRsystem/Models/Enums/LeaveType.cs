namespace HRsystem.Models.Enums
{
    public enum LeaveType
    {
        None = 0,
        /// <summary>إجازة سنوية</summary>
        Annual = 1,
        /// <summary>إجازة عارضة (مناسبات)</summary>
        Casual = 2,
        /// <summary>إجازة مرضية</summary>
        Sick = 3,
        /// <summary>إجازة حج</summary>
        Hajj = 4,
        /// <summary>إجازة أمومة</summary>
        Maternity = 5,
        /// <summary>إجازة بدون راتب</summary>
        Unpaid = 6,
        /// <summary>إجازة تعويضي (بدل عطلة)</summary>
        Compensatory = 7,
        /// <summary>إجازة رسمية (عيد/وطني)</summary>
        OfficialHoliday = 8,
        /// <summary>إجازة اختبارات</summary>
        Exam = 9
    }
}