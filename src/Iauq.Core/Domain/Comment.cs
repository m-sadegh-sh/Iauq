using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Iauq.Core.Domain
{
    public class Comment : EntityBase
    {
        [DisplayName("وضعیت تایید")]
        public virtual bool IsApproved { get; set; }

        [DisplayName("تاریخ ثبت")]
        public virtual DateTime CommentDate
        {
            get { return new DateTime(CommentDateTicks); }
            set { CommentDateTicks = value.Ticks; }
        }

        public virtual long CommentDateTicks { get; set; }

        [DisplayName("امتیاز ها (مثبت)")]
        public virtual int PositiveRates { get; set; }

        [DisplayName("امتیاز ها (منفی)")]
        public virtual int NagetiveRates { get; set; }

        [DisplayName("عنوان")]
        [Required(ErrorMessage = "ذکر عنوان ضروری است.")]
        [StringLength(100, ErrorMessage = "حداکثر طول عنوان 100 کاراکتر است.")]
        public virtual string Title { get; set; }

        [DisplayName("نظر")]
        [Required(ErrorMessage = "ذکر نظر ضروری است.")]
        [StringLength(2000, ErrorMessage = "حداکثر طول نظر 2000 کاراکتر است.")]
        public virtual string Body { get; set; }

        public virtual int? ParentId { get; set; }

        [DisplayName("در پاسخ به")]
        public virtual Comment Parent { get; set; }

        public virtual int? CommentorId { get; set; }

        [DisplayName("نظر دهنده")]
        public virtual User Commentor { get; set; }

        public virtual string CommentorIp { get; set; }

        public virtual int OwnerId { get; set; }

        [DisplayName("مربوط به")]
        public virtual Content Owner { get; set; }

        [DisplayName("فرزندان")]
        public virtual ICollection<Comment> Childs { get; set; }
    }
}