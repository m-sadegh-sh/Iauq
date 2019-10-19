using System;
using System.Collections.Generic;

namespace Iauq.Core.Domain
{
    public class Site : EntityBase
    {
        public string Key { get; set; }

        public string OwnerId { get; set; }

        public int ParentId { get; set; }

        public Site Parent { get; set; }

        public virtual ICollection<Site> Childs { get; set; }
        
        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public short TypeShort { get; set; }

        public SiteType Type
        {
            get { return (SiteType) TypeShort; }
            set { TypeShort = (short) value; }
        }

        public string SourceId { get; set; }

        public long LastLoginTicks { get; set; }

        public DateTime LastLoginDate
        {
            get { return new DateTime(LastLoginTicks); }
            set { LastLoginTicks = value.Ticks; }
        }

        public int ThemeId { get; set; }

        public bool ShowMessageBox { get; set; }

        public string AdminId { get; set; }

        public string Email { get; set; }
        
        public virtual ICollection<Module> Modules { get; set; }
        
        public virtual ICollection<Content> Contents { get; set; }

        public virtual ICollection<Message> Messages { get; set; }
    }
}