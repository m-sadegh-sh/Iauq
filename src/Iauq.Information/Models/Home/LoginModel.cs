
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
namespace Iauq.Information.Models.Home
{
    public class LoginModel : ModelBase
    {
        [DisplayName("نام کاربری")]
        [Required(ErrorMessage = "نام کاربری را وارد نمایید.")]
        [StringLength(32, ErrorMessage = "حداکثر طول نام کاربری 32 کاراکتر است.")]
        public string UserName { get; set; }

        [DisplayName("رمز عبور")]
        [Required(ErrorMessage = "رمز عبور را وارد نمایید.")]
        [StringLength(16, ErrorMessage = "حداکثر طول رمز عبور 16 کاراکتر است.")]
        public string Password { get; set; }

        public string SecurityToken { get; set; }

        [DisplayName("تصویر امنیتی")]
        [Required(ErrorMessage = "تصویر امنیتی را وارد نمایید.")]
        [StringLength(10, ErrorMessage = "حداکثر طول تصویر امنیتی 10 کاراکتر است.")]
        public string Captcha { get; set; }

        [DisplayName("مرا را بخاطر بسپار!")]
        public bool RemmeberMe { get; set; }

        public string Done { get; set; }
    }
}