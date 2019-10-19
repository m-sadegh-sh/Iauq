namespace Iauq.Core.Domain
{
    public class Module : EntityBase
    {
        public int OwnerId { get; set; }
        public Site Owner { get; set; }
        
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }

        public short PositionShort { get; set; }

        public ModulePosition Position
        {
            get { return (ModulePosition) PositionShort; }
            set { PositionShort = (short) value; }
        }

        public int Order { get; set; }
        public bool Visible { get; set; }
        public bool Deleted { get; set; }
    }
}