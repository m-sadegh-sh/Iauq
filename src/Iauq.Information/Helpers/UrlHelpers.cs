using System;
using System.Web.Mvc;
using Iauq.Core.Domain;

namespace Iauq.Information.Helpers
{
    public static class UrlHelpers
    {
        public static string TopicUrl(this UrlHelper helper, Content topic)
        {
            switch (topic.Type)
            {
                case ContentType.News:
                    return NewsUrl(helper, topic);
                case ContentType.Events:
                    return EventUrl(helper, topic);
                case ContentType.Calendars:
                    return CalendarUrl(helper, topic);
                case ContentType.Pages:
                case ContentType.Menu:
                    return PageUrl(helper, topic);
                case ContentType.Links:
                    return LinkUrl(helper, topic);
            }

            return null;
        }

        public static string NewsUrl(this UrlHelper urlHelper, Content news)
        {
            if (String.IsNullOrEmpty(news.Metadata.SeoSlug))
                return urlHelper.Action("Details", "News", new {news.Id});

            return urlHelper.Action("Details", "News", new {news.Id, slug = news.Metadata.SeoSlug});
        }

        public static string EventUrl(this UrlHelper urlHelper, Content @event)
        {
            if (String.IsNullOrEmpty(@event.Metadata.SeoSlug))
                return urlHelper.Action("Details", "Events", new {@event.Id});

            return urlHelper.Action("Details", "Events", new {@event.Id, slug = @event.Metadata.SeoSlug});
        }

        public static string PollUrl(this UrlHelper urlHelper, Poll poll)
        {
            return urlHelper.Action("Details", "Polls", new {poll.Id});
        }

        public static string CalendarUrl(this UrlHelper urlHelper, Content calendar)
        {
            if (String.IsNullOrEmpty(calendar.Metadata.SeoSlug))
                return urlHelper.Action("Details", "Calendars", new {calendar.Id});

            return urlHelper.Action("Details", "Calendars", new {calendar.Id, slug = calendar.Metadata.SeoSlug});
        }

        public static string PageUrl(this UrlHelper urlHelper, Content page)
        {
            if (String.IsNullOrEmpty(page.Metadata.SeoSlug))
                return urlHelper.Action("Details", "Pages", new {page.Id});

            return urlHelper.Action("Details", "Pages", new {page.Id, slug = page.Metadata.SeoSlug});
        }

        public static string LinkUrl(this UrlHelper urlHelper, Content link)
        {
            return link.Body.Trim();
        }
    }
}