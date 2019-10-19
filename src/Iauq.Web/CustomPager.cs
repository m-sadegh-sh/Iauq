using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using MvcContrib.Pagination;

namespace Iauq.Web
{
    public class CustomPager : IHtmlString
    {
        private const string PaginationFirst = "ابتدا";

        private const string PaginationFormat =
            "<li class='disabled'><a class='last-child' trap='true'>نمایش {0} تا {1} از {2}&nbsp;</a></li>";

        private const string PaginationLast = "انتها";
        private const string PaginationNext = "بعدی";
        private const string PaginationPrev = "قبلی";

        private const string PaginationSingleFormat =
            "<li class='disabled'><a class='last-child' trap='true'>نمایش {0} از {1}&nbsp;</a></li>";

        private readonly IPagination _pagination;
        private readonly Func<int, string> _urlBuilder;
        private readonly ViewContext _viewContext;

        public CustomPager(IPagination pagination, ViewContext context, Func<int, string> urlBuilder)
        {
            _pagination = pagination;
            _viewContext = context;
            _urlBuilder = urlBuilder;
        }

        protected ViewContext ViewContext
        {
            get { return _viewContext; }
        }

        #region IHtmlString Members

        public string ToHtmlString()
        {
            if (_pagination.TotalItems == 0)
            {
                return null;
            }

            var builder = new StringBuilder();

            builder.Append("<div class='pagination'>");
            RenderLeftSideOfPager(builder);

            if (_pagination.TotalPages > 1)
            {
                RenderRightSideOfPager(builder);
            }

            builder.Append("<div class='clearfix'></div></div>");

            return builder.ToString();
        }

        #endregion

        public override string ToString()
        {
            return ToHtmlString();
        }

        protected virtual void RenderLeftSideOfPager(StringBuilder builder)
        {
            builder.Append("<ul class='pull-right'>");

            if (_pagination.PageSize == 1)
            {
                RenderNumberOfItemsWhenThereIsOnlyOneItemPerPage(builder);
            }
            else
            {
                RenderNumberOfItemsWhenThereAreMultipleItemsPerPage(builder);
            }

            builder.Append("</ul>");
        }

        protected virtual void RenderRightSideOfPager(StringBuilder builder)
        {
            builder.Append("<ul class='pull-left'>");

            if (_pagination.PageNumber == 1)
            {
                builder.Append("<li class=\"active\"><a class=\"first-child\" trap=\"true\">" + PaginationFirst +
                               "</a></li>");
            }
            else
            {
                builder.Append(CreatePageLink(1, PaginationFirst));
            }

            if (_pagination.HasPreviousPage)
            {
                builder.Append(CreatePageLink(_pagination.PageNumber - 1, PaginationPrev));
            }
            else
            {
                builder.Append("<li class=\"active\"><a trap=\"true\">" + PaginationPrev + "</a></li>");
            }

            if (_pagination.HasNextPage)
            {
                builder.Append(CreatePageLink(_pagination.PageNumber + 1, PaginationNext));
            }
            else
            {
                builder.Append("<li class=\"active\"><a trap=\"true\">" + PaginationNext + "</a></li>");
            }

            int lastPage = _pagination.TotalPages;

            if (_pagination.PageNumber < lastPage)
            {
                builder.Append(CreatePageLink(lastPage, PaginationLast, true));
            }
            else
            {
                builder.Append("<li class=\"active\"><a class=\"last-child\" trap=\"true\">" + PaginationLast +
                               "</a></li>");
            }

            builder.Append("</ul>");
        }


        protected virtual void RenderNumberOfItemsWhenThereIsOnlyOneItemPerPage(StringBuilder builder)
        {
            builder.AppendFormat(PaginationSingleFormat, _pagination.FirstItem, _pagination.TotalItems);
        }

        protected virtual void RenderNumberOfItemsWhenThereAreMultipleItemsPerPage(StringBuilder builder)
        {
            builder.AppendFormat(PaginationFormat, _pagination.FirstItem, _pagination.LastItem, _pagination.TotalItems);
        }

        private string CreatePageLink(int pageNumber, string text, bool isLastChild = false)
        {
            var builder = new TagBuilder("a");
            builder.SetInnerText(text);
            builder.AddCssClass(pageNumber == 1 ? "first-child" : isLastChild ? "last-child" : "");
            builder.MergeAttribute("href", _urlBuilder(pageNumber));
            return "<li>" + builder.ToString(TagRenderMode.Normal) + "</li>";
        }
    }
}