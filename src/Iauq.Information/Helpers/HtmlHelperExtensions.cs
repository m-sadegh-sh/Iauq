using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace Iauq.Information.Helpers
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString RenderTags(this HtmlHelper helper, string tags, string nullText)
        {
            if (string.IsNullOrEmpty(tags))
                return new MvcHtmlString(nullText);

            List<string> tagsList = tags.Split(new[] {',', ';', '،'}).ToList();

            if (tags == nullText || tags.Length == 0)
                return new MvcHtmlString(nullText);

            var stringBuilder = new StringBuilder();

            bool isFirst = true;

            foreach (string tag in tagsList)
            {
                if (isFirst)
                    isFirst = false;
                else
                    stringBuilder.Append(", ");

                stringBuilder.Append(helper.ActionLink(tag.Trim(), "Search", "Home", new {tag=tag.Trim(), page = 1}, null));
            }

            return new MvcHtmlString(stringBuilder.ToString());
        }

        public static MvcHtmlString Image(this HtmlHelper helper,
                                          string url,
                                          string altText,
                                          object htmlAttributes)
        {
            var builder = new TagBuilder("img");
            builder.Attributes.Add("src", url);
            builder.Attributes.Add("alt", altText);
            builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            return new MvcHtmlString(builder.ToString(TagRenderMode.SelfClosing));
        }
    }
}