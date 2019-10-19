using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Iauq.Core.Domain
{
    public class ChoiceItem : EntityBase
    {
        public virtual int OwnerId { get; set; }
        public virtual Choice Owner { get; set; }

        [DisplayName("متن آیتم")]
        [Required(ErrorMessage = "ذکر متن آیتم ضروری است.")]
        [StringLength(200, ErrorMessage = "حداکثر طول متن آیتم 200 کاراکتر است.")]
        public virtual string Text { get; set; }

        public virtual ICollection<Answer> Answers { get; set; }
    }
}