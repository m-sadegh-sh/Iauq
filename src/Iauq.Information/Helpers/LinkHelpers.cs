using System.Web.Mvc;
using System.Web.Mvc.Html;
using Iauq.Core.Domain;
using Iauq.Core.Extensions;

namespace Iauq.Information.Helpers
{
    public static class LinkHelpers
    {
        public static MvcHtmlString TopicLink(this HtmlHelper htmlHelper, Content topic)
        {
            switch (topic.Type)
            {
                case ContentType.News:
                    return NewsLink(htmlHelper, topic);
                case ContentType.Events:
                    return EventLink(htmlHelper, topic);
                case ContentType.Calendars:
                    return CalendarLink(htmlHelper, topic);
                case ContentType.Pages:
                case ContentType.Menu:
                    return PageLink(htmlHelper, topic);
                case ContentType.Links:
                    return LinkLink(htmlHelper, topic);
            }

            return null;
        }

        public static MvcHtmlString NewsLink(this HtmlHelper htmlHelper, Content news, string title = null)
        {
            if (string.IsNullOrEmpty(news.Metadata.SeoSlug))
                return htmlHelper.ActionLink(title ?? "ادامه", "Details", "News",
                                             new {news.Id}, null);

            return htmlHelper.ActionLink(title ?? "ادامه", "Details", "News",
                                         new {news.Id, slug = news.Metadata.SeoSlug}, null);
        }

        public static MvcHtmlString EventLink(this HtmlHelper htmlHelper, Content @event, string title = null)
        {
            if (string.IsNullOrEmpty(@event.Metadata.SeoSlug))
                return htmlHelper.ActionLink(title ?? "جزئیات", "Details", "Events", new {@event.Id},
                                             null);

            return htmlHelper.ActionLink(title ?? "جزئیات", "Details", "Events",
                                         new {@event.Id, slug = @event.Metadata.SeoSlug},
                                         null);
        }

        public static MvcHtmlString PollLink(this HtmlHelper htmlHelper, Poll poll, string title = null)
        {
            return htmlHelper.ActionLink(title ?? "جزئیات", "Details", "Polls", new {poll.Id},
                                         null);
        }

        public static MvcHtmlString CalendarLink(this HtmlHelper htmlHelper, Content calendar, string title = null)
        {
            if (string.IsNullOrEmpty(calendar.Metadata.SeoSlug))
                return htmlHelper.ActionLink(title ?? "جزئیات", "Details", "Calendars", new {calendar.Id},
                                             null);

            return htmlHelper.ActionLink(title ?? "جزئیات", "Details", "Calendars",
                                         new {calendar.Id, slug = calendar.Metadata.SeoSlug},
                                         null);
        }

        public static MvcHtmlString PageLink(this HtmlHelper htmlHelper, Content page)
        {
            if (page.Type == ContentType.Links)
                return LinkLink(htmlHelper, page);

            if (string.IsNullOrEmpty(page.Metadata.SeoSlug))
                return htmlHelper.ActionLink(page.Title.EnsureLength(80), "Details", "Pages",
                                             new {page.Id},
                                             null);

            return htmlHelper.ActionLink(page.Title.EnsureLength(80), "Details", "Pages",
                                         new {page.Id, slug = page.Metadata.SeoSlug}, null);
        }

        public static MvcHtmlString LinkLink(this HtmlHelper htmlHelper, Content link)
        {
            return
                new MvcHtmlString(string.Format("<a href=\"{0}\">{1}</a>", link.Body.Trim(),
                                                link.Title.EnsureLength(80)));
        }
    }
}