using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Iauq.Information.Models;

namespace Iauq.Information.Areas.Administration.Models.Administration
{
    public class ChangePasswordModel : ModelBase
    {
        [DisplayName("رمز عبور قبلی")]
        [Required(ErrorMessage = "رمز عبور را وارد نمایید.")]
        [StringLength(16, ErrorMessage = "حداکثر طول رمز عبور قبلی 16 کاراکتر است.")]
        public string OldPassword { get; set; }

        [DisplayName("رمز عبور جدید")]
        [Required(ErrorMessage = "رمز عبور جدید را وارد نمایید.")]
        [StringLength(16, ErrorMessage = "حداکثر طول رمز عبور جدید 16 کاراکتر است.")]
        public string NewPassword { get; set; }

        [DisplayName("تائید رمز عبور جدید")]
        [Required(ErrorMessage = "تائید رمز عبور جدید را وارد نمایید.")]
        [StringLength(16, ErrorMessage = "حداکثر طول تائید رمز عبور جدید 16 کاراکتر است.")]
        [Compare("NewPassword", ErrorMessage = "رمز عبور جدید و تائید رمز عبور جدید با یکدیگر همخوانی ندارند.")]
        public string NewPasswordConfirmation { get; set; }

        public string SecurityToken { get; set; }
    }
}