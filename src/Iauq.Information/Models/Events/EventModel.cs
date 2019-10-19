using FarsiLibrary.Utils;

namespace Iauq.Information.Models.Events
{
    public class EventModel : ModelBase
    {
        public long Id { get; set; }
        public PersianDate HoldingDate { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
    }
}