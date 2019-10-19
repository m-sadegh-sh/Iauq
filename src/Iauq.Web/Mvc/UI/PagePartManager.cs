using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using FarsiLibrary.Utils;

namespace Iauq.Web.Mvc.UI
{
    public class PagePartManager
    {
        private const string PartKey = "$parts";

        private readonly List<MvcHtmlString> _breadcrumbs;

        public PagePartManager()
        {
            _breadcrumbs = new List<MvcHtmlString>();
        }

        public static PagePartManager Current
        {
            get
            {
                var parts = HttpContext.Current.Items[PartKey] as PagePartManager;

                if (parts == null)
                {
                    HttpContext.Current.Items[PartKey] = parts = new PagePartManager();
                }

                return parts;
            }
        }

        public void AddBreadcrumb(MvcHtmlString link)
        {
            _breadcrumbs.Add(link);
        }

        public MvcHtmlString RenderBreadcrumbs()
        {
            var output = new StringBuilder();

            output.Append("<div class=\"breadcrumb hidden-small-phone\">");

            for (int i = 0; i < _breadcrumbs.Count; i++)
            {
                output.Append("<li>");

                output.Append(_breadcrumbs[i]);

                if (i != _breadcrumbs.Count - 1)
                    output.Append("<span class=\"divider\">/</span>");

                output.Append("</li>");
            }

            if (HttpContext.Current.User.Identity.IsAuthenticated)
                output.AppendFormat("<span class=\"pull-left\">{1} عزیز خوش آمدید. / {0}</span></div>",
                                    PersianDate.Now.ToWritten(), HttpContext.Current.User.Identity.Name);
            else
                output.AppendFormat("<span class=\"pull-left\">{0}</span></div>",
                                    PersianDate.Now.ToWritten());

            return new MvcHtmlString(output.ToString());
        }

        public string RenderTitle(string title)
        {
            var output = new StringBuilder();

            output.Append("دانشگاه آزاد اسلامی، واحد قوچان / ");

            for (int i = 0; i < _breadcrumbs.Count; i++)
            {
                string text = _breadcrumbs[i].ToString();
                text = Regex.Replace(text, "<.*?>", "");

                output.Append(text);

                if (title != null || i != _breadcrumbs.Count - 1)
                    output.Append(" / ");
            }

            if (title != null)
                output.Append(title);

            return output.ToString();
        }
    }
}