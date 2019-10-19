using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;

namespace Iauq.Core.Domain
{
    public class User : EntityBase
    {
        [DisplayName("نام کاربری")]
        [Required(ErrorMessage = "ذکر نام کاربری ضروری است.")]
        [StringLength(32, ErrorMessage = "حداکثر طول نام کاربری 32 کاراکتر است.")]
        public virtual string UserName { get; set; }

        [DisplayName("گذرواژه")]
        [Required(ErrorMessage = "ذکر گذرواژه ضروری است.")]
        [StringLength(16, ErrorMessage = "حداکثر طول گذرواژه 16 کاراکتر است.")]
        public virtual string Password { get; set; }

        public virtual string Salt { get; set; }

        [DisplayName("نشانه امنیتی")]
        [Required(ErrorMessage = "ذکر نشانه امنیتی ضروری است.")]
        [StringLength(32, ErrorMessage = "حداکثر طول نشانه امنیتی 32 کاراکتر است.")]
        public virtual string SecurityToken { get; set; }

        [DisplayName("پست الکترونیک")]
        [Required(ErrorMessage = "ذکر پست الکترونیک ضروری است.")]
        [StringLength(128, ErrorMessage = "حداکثر طول پست الکترونیک 128 کاراکتر است.")]
        [Email(ErrorMessage = "پست الکترونیک وارد شده دارای قالب صحیح نمی باشد.")]
        public virtual string Email { get; set; }

        [DisplayName("نقش ها")]
        public virtual ICollection<Role> Roles { get; set; }

        public virtual ICollection<Content> Contents { get; set; }

        public virtual ICollection<File> Files { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public virtual ICollection<Answer> Answers { get; set; }
    }
}