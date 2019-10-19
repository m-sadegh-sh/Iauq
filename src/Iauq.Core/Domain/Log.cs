using System;
using System.ComponentModel;

namespace Iauq.Core.Domain
{
    public class Log : EntityBase
    {
        [DisplayName("نوع")]
        public virtual LogLevel Level
        {
            get { return (LogLevel) LevelShort; }
            set { LevelShort = (short) value; }
        }

        public virtual short LevelShort { get; set; }

        [DisplayName("پیام")]
        public virtual string Message { get; set; }

        [DisplayName("استک")]
        public virtual string Stack { get; set; }

        [DisplayName("نوع استثنا")]
        public virtual string ExceptionType { get; set; }

        [DisplayName("آدرس (درخواست)")]
        public virtual string RequestUrl { get; set; }

        [DisplayName("آدرس (ارجاع)")]
        public virtual string ReferUrl { get; set; }

        [DisplayName("آی پی آدرس")]
        public virtual string IpAddress { get; set; }

        [DisplayName("تاریخ ثبت")]
        public virtual DateTime LogDate { get; set; }
    }
}