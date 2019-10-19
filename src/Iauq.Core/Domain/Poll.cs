using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Iauq.Core.Domain
{
    public class Poll : EntityBase
    {
        [DisplayName("وضعیت (فعال)")]
        public virtual bool IsActive { get; set; }

        [DisplayName("نمایش در صفحه اصلی")]
        public virtual bool ShowOnHomePage { get; set; }

        [DisplayName("تاریخ انتشار")]
        [NotMapped]
        public virtual DateTime CreateDate
        {
            get { return new DateTime(CreateDateTicks); }
            set { CreateDateTicks = value.Ticks; }
        }

        public virtual long CreateDateTicks { get; set; }

        [DisplayName("عنوان")]
        [Required(ErrorMessage = "ذکر عنوان ضروری است.")]
        [StringLength(200, ErrorMessage = "حداکثر طول عنوان 200 کاراکتر است.")]
        public virtual string Title { get; set; }

        [DisplayName("توضیحات")]
        public virtual string Description { get; set; }

        public virtual ICollection<Choice> Choices { get; set; }
    }
}