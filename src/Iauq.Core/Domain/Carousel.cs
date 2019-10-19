using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Iauq.Core.Domain
{
    public class Carousel : EntityBase
    {
        [DisplayName("ترتیب نمایش")]
        public virtual short DisplayOrder { get; set; }

        [DisplayName("عنوان")]
        [Required(ErrorMessage = "ذکر عنوان ضروری است.")]
        [StringLength(200, ErrorMessage = "حداکثر طول عنوان 200 کاراکتر است.")]
        public virtual string Title { get; set; }

        [DisplayName("توضیحات")]
        [StringLength(1000, ErrorMessage = "حداکثر طول توضیحات 1000 کاراکتر است.")]
        public virtual string Description { get; set; }

        [DisplayName("آدرس")]
        [StringLength(2000, ErrorMessage = "حداکثر طول آدرس 2000 کاراکتر است.")]
        public virtual string LinkUrl { get; set; }

        [DisplayName("عکس")]
        public HttpPostedFileBase Slide { get; set; }
    }
}