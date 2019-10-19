using System.Collections.Generic;
using FarsiLibrary.Utils;

namespace Iauq.Information.Models.Calendars
{
    public class CalendarsModel : List<CalendarModel>
    {
        public CalendarsModel()
        {
            
        }
        public CalendarsModel Add(PersianDate scheduleDate, string title)
        {
            Add(new CalendarModel
                    {
                        ScheduleDate = scheduleDate,
                        Title = title
                    });

            return this;
        }
    }
}