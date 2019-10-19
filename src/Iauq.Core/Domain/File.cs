using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web;

namespace Iauq.Core.Domain
{
    public class File : EntityBase
    {
        public File()
        {
            Guid = Guid.NewGuid();
        }

        public bool IsDirectory
        {
            get { return string.IsNullOrEmpty(ContentType); }
        }

        public bool IsFile
        {
            get { return !string.IsNullOrEmpty(ContentType); }
        }

        public virtual Guid Guid { get; set; }

        [DisplayName("وضعیت انتشار")]
        public virtual bool IsPublished { get; set; }

        [DisplayName("نحوه دسترسی")]
        public virtual AccessMode AccessMode
        {
            get { return (AccessMode) AccessModeShort; }
            set { AccessModeShort = (short) value; }
        }

        public virtual short AccessModeShort { get; set; }

        [DisplayName("تاریخ ایجاد")]
        public virtual DateTime CreateDate
        {
            get { return new DateTime(CreateDateTicks); }
            set { CreateDateTicks = value.Ticks; }
        }

        public virtual long CreateDateTicks { get; set; }

        [DisplayName("دفعات دسترسی")]
        public virtual long AccessCount { get; set; }

        [DisplayName("نام")]
        public virtual string Name { get; set; }

        [DisplayName("نوع محتوا")]
        public virtual string ContentType { get; set; }

        [DisplayName("حجم")]
        public virtual long Size { get; set; }

        [DisplayName("ارسال کننده")]
        public virtual int UploaderId { get; set; }

        public virtual User Uploader { get; set; }

        [DisplayName("والد")]
        public virtual int? ParentId { get; set; }

        public virtual File Parent { get; set; }

        public virtual ICollection<File> Childs { get; set; }

        public long GetFullSize()
        {
            if (ContentType != null)
                return Size;

            return Childs.Sum(c => c.GetFullSize());
        }
    }
}