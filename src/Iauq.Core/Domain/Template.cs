using System;
using System.ComponentModel;

namespace Iauq.Core.Domain
{
    public class Template : EntityBase
    {

        [DisplayName("عنوان")]
        public virtual string Title { get; set; }

        [DisplayName("توضیح")]
        public virtual string Description { get; set; }

        [DisplayName("محتوا")]
        public virtual string Body { get; set; }
    }
}