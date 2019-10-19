using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Iauq.Core.Domain
{
    public class Content : EntityBase
    {
        public virtual bool IsPublished { get; set; }

        [DisplayName("نمایش فرزندان")]
        public virtual bool ShowNestedContents { get; set; }

        [DisplayName("مطلب داغ")]
        public virtual bool IsHot { get; set; }

        public virtual ContentType Type
        {
            get { return (ContentType) TypeInt; }
            set { TypeInt = (short) value; }
        }

        [DisplayName("نوع مطلب")]
        public virtual short TypeInt { get; set; }

        [DisplayName("ترتیب نمایش")]
        public virtual short DisplayOrder { get; set; }

        [DisplayName("تاریخ انتشار")]
        public virtual DateTime PublishDate
        {
            get { return new DateTime(PublishDateTicks); }
            set { PublishDateTicks = value.Ticks; }
        }

        public virtual long PublishDateTicks { get; set; }

        [DisplayName("امتیاز")]
        public virtual double Rate { get; set; }

        [DisplayName("تعداد بازدید")]
        public virtual int PageViews { get; set; }

        [DisplayName("عنوان")]
        [Required(ErrorMessage = "ذکر عنوان ضروری است.")]
        [StringLength(200, ErrorMessage = "حداکثر طول عنوان 200 کاراکتر است.")]
        public virtual string Title { get; set; }

        [DisplayName("خلاصه")]
        [StringLength(1000, ErrorMessage = "حداکثر طول خلاصه 1000 کاراکتر است.")]
        public virtual string Abstract { get; set; }

        [DisplayName("متن کامل")]
        [Required(ErrorMessage = "ذکر متن کامل ضروری است.")]
        public virtual string Body { get; set; }

        [DisplayName("برچسب ها")]
        public virtual string Tags { get; set; }

        public virtual SeoMetadata Metadata { get; set; }

        [DisplayName("زیر دامنه")]
        //public virtual int SiteId { get; set; }

        //public virtual Site Site { get; set; }
        public virtual int? CategoryId { get; set; }

        public virtual Category Category { get; set; }

        [DisplayName("والد")]
        public virtual int? ParentId { get; set; }

        public virtual Content Parent { get; set; }

        [DisplayName("توسط")]
        public virtual int AuthorId { get; set; }

        public virtual User Author { get; set; }

        public virtual ICollection<Content> Childs { get; set; }

        [DisplayName("نظرات")]
        public virtual ICollection<Comment> Comments { get; set; }
    }
}