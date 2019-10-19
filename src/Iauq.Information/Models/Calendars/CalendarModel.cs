using FarsiLibrary.Utils;

namespace Iauq.Information.Models.Calendars
{
    public class CalendarModel : ModelBase
    {
        public PersianDate ScheduleDate { get; set; }
        public string Title { get; set; }
    }
}