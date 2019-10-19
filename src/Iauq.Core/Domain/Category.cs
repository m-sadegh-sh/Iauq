using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Iauq.Core.Domain
{
    public class Category : EntityBase
    {
        [DisplayName("ترتیب نمایش")]
        public virtual short DisplayOrder { get; set; }

        [DisplayName("عنوان")]
        [Required(ErrorMessage = "ذکر عنوان ضروری است.")]
        [StringLength(200, ErrorMessage = "حداکثر طول عنوان 200 کاراکتر است.")]
        public virtual string Title { get; set; }

        [DisplayName("زبان")]
        public virtual int LanguageId { get; set; }

        public virtual Language Language { get; set; }

        //public virtual ICollection<Site> Sites { get; set; }
        public virtual ICollection<Content> Contents { get; set; }

        [DisplayName("والد")]
        public virtual int? ParentId { get; set; }

        public virtual Category Parent { get; set; }

        public virtual ICollection<Category> Childs { get; set; }

        public virtual ICollection<Role> Roles { get; set; }
    }
}