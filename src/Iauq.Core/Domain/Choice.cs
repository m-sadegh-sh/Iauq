using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Iauq.Core.Domain
{
    public class Choice : EntityBase
    {
        public virtual int OwnerId { get; set; }
        public virtual Poll Owner { get; set; }

        [DisplayName("عنوان")]
        [Required(ErrorMessage = "ذکر عنوان ضروری است.")]
        [StringLength(200, ErrorMessage = "حداکثر طول عنوان 200 کاراکتر است.")]
        public virtual string Title { get; set; }

        [DisplayName("توضیحات")]
        public virtual string Description { get; set; }

        [DisplayName("آیا امکان انتخاب چند گزینه وجود داشته باشد؟")]
        public virtual bool IsMultiSelection { get; set; }

        public virtual ICollection<ChoiceItem> Items { get; set; }
    }
}