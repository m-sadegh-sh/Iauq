using System;

namespace Iauq.Core.Domain
{
    public class Message : EntityBase
    {
        public int OwnerId { get; set; }
        public Site Owner { get; set; }

        public string SenderName { get; set; }
        public string SenderId { get; set; }
        public string SenderPhone { get; set; }
        public string SenderEmail { get; set; }

        public long SentTicks { get; set; }

        public DateTime SentDate
        {
            get { return new DateTime(SentTicks); }
            set { SentTicks = value.Ticks; }
        }

        public string Subject { get; set; }
        public string Body { get; set; }
    }
}