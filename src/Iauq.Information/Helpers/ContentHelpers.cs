using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Iauq.Core.Domain;
using Iauq.Core.Extensions;
using Iauq.Web;
using MvcContrib.Pagination;

namespace Iauq.Information.Helpers
{
    public static class ContentHelpers
    {
        public static CustomPager RenderPager<T>(this HtmlHelper htmlHelper, T model, Func<int, string> urlBuilder)
            where T : IPagination
        {
            return new CustomPager(model, htmlHelper.ViewContext, urlBuilder);
        }

        public static MvcHtmlString TopicTitle(this HtmlHelper htmlHelper, Content topic)
        {
            switch (topic.Type)
            {
                case ContentType.News:
                    return NewsTitle(htmlHelper, topic);
                case ContentType.Events:
                    return EventTitle(htmlHelper, topic);
                case ContentType.Calendars:
                    return CalendarTitle(htmlHelper, topic);
                case ContentType.Pages:
                    return PageTitle(htmlHelper, topic);
                case ContentType.Links:
                    return LinkTitle(htmlHelper, topic);
            }

            return null;
        }

        public static MvcHtmlString NewsTitle(this HtmlHelper htmlHelper, Content news)
        {
            if (string.IsNullOrEmpty(news.Metadata.SeoSlug))
                return
                    htmlHelper.ActionLink(news.Title.EnsureLength(80), "Details", "News",
                                          new {news.Id}, null);

            return
                htmlHelper.ActionLink(news.Title.EnsureLength(80), "Details", "News",
                                      new {news.Id, slug = news.Metadata.SeoSlug}, null);
        }

        public static MvcHtmlString EventTitle(this HtmlHelper htmlHelper, Content @event)
        {
            if (string.IsNullOrEmpty(@event.Metadata.SeoSlug))
                return
                    htmlHelper.ActionLink(@event.Title.EnsureLength(80), "Details", "Events", new {@event.Id},
                                          null);
            return
                htmlHelper.ActionLink(@event.Title.EnsureLength(80), "Details", "Events",
                                      new {@event.Id, slug = @event.Metadata.SeoSlug},
                                      null);
        }

        public static MvcHtmlString PollTitle(this HtmlHelper htmlHelper, Poll poll)
        {
            return
                htmlHelper.ActionLink(poll.Title.EnsureLength(80), "Details", "Polling", new {poll.Id}, null);
        }

        public static MvcHtmlString CalendarTitle(this HtmlHelper htmlHelper, Content calendar)
        {
            if (string.IsNullOrEmpty(calendar.Metadata.SeoSlug))
                return
                    htmlHelper.ActionLink(calendar.Title.EnsureLength(80), "Details", "Calendars", new {calendar.Id},
                                          null);
            return
                htmlHelper.ActionLink(calendar.Title.EnsureLength(80), "Details", "Calendars",
                                      new {calendar.Id, slug = calendar.Metadata.SeoSlug},
                                      null);
        }

        public static MvcHtmlString PageTitle(this HtmlHelper htmlHelper, Content page)
        {
            if (page.Type == ContentType.Links)
                return LinkTitle(htmlHelper, page);

            if (string.IsNullOrEmpty(page.Metadata.SeoSlug))
                return
                    htmlHelper.ActionLink(page.Title.EnsureLength(80), "Details", "Pages", new {page.Id},
                                          null);
            return
                htmlHelper.ActionLink(page.Title.EnsureLength(80), "Details", "Pages",
                                      new {page.Id, slug = page.Metadata.SeoSlug},
                                      null);
        }

        public static MvcHtmlString LinkTitle(this HtmlHelper htmlHelper, Content link)
        {
            return new MvcHtmlString(string.Format("<a href=\"{0}\">\"{1}\"</a>", link.Body.Trim(), link.Title));
        }
    }
}