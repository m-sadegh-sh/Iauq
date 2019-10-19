using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Iauq.Core.Domain
{
    public class SeoMetadata
    {
        [DisplayName("عنوان (سئو)")]
        [StringLength(300, ErrorMessage = "حداکثر طول عنوان (سئو) 300 کاراکتر است.")]
        public string SeoTitle { get; set; }

        [DisplayName("اسلاگ (سئو)")]
        [StringLength(300, ErrorMessage = "حداکثر طول اسلاگ (سئو) 300 کاراکتر است.")]
        public string SeoSlug { get; set; }

        [DisplayName("کلمات کلیدی (سئو)")]
        [StringLength(200, ErrorMessage = "حداکثر طول کلمات کلیدی (سئو) 200 کاراکتر است.")]
        public string SeoKeywords { get; set; }

        [DisplayName("توضیحات (سئو)")]
        [StringLength(600, ErrorMessage = "حداکثر طول توضیحات (سئو) 600 کاراکتر است.")]
        public string SeoDescription { get; set; }
    }
}