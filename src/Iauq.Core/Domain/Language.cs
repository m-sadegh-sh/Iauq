using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Iauq.Core.Domain
{
    public class Language : EntityBase
    {
        [DisplayName("نام")]
        [Required(ErrorMessage = "ذکر نام ضروری است.")]
        [StringLength(200, ErrorMessage = "حداکثر طول نام 200 کاراکتر است.")]
        public virtual string Name { get; set; }

        [DisplayName("کد")]
        [Required(ErrorMessage = "ذکر کد ضروری است.")]
        [StringLength(2, ErrorMessage = "حداکثر طول کد 2 کاراکتر است.")]
        public virtual string IsoCode { get; set; }

        public virtual ICollection<Category> Categories { get; set; }

        public virtual ICollection<Content> Contents { get; set; }
    }
}