using System.ComponentModel.DataAnnotations;

namespace HRsystem.ViewModels
{
    public class ChangePasswordViewModel
    {


        [Required(ErrorMessage = "كلمة المرور الجديدة مطلوبة")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "كلمة المرور يجب أن تكون على الأقل 6 أحرف")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "تأكيد كلمة المرور مطلوبة")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "كلمة المرور الجديدة وتأكيدها غير متطابقين")]
        public string ConfirmNewPassword { get; set; }
    }
}