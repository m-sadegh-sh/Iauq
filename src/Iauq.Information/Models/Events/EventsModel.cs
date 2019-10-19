using System.Collections.Generic;
using FarsiLibrary.Utils;

namespace Iauq.Information.Models.Events
{
    public class EventsModel : List<EventModel>
    {
        public EventsModel Add(long id, PersianDate holdingDate, string title, string slug, string description)
        {
            Add(new EventModel
                    {
                        Id = id,
                        HoldingDate = holdingDate,
                        Title = title,
                        Slug = slug,
                        Description = description
                    });

            return this;
        }
    }
}