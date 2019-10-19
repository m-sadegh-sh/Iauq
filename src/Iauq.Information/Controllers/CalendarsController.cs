using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Iauq.Core.Domain;
using Iauq.Data;
using Iauq.Data.Services;
using Iauq.Information.Helpers;
using MvcContrib.Pagination;

namespace Iauq.Information.Controllers
{
    public class CalendarsController : ControllerBase
    {
        private readonly IContentService _contentService;
        private readonly IUnitOfWork _unitOfWork;

        public CalendarsController(IContentService contentService, IUnitOfWork unitOfWork)
        {
            _contentService = contentService;
            _unitOfWork = unitOfWork;
        }

        public ActionResult Archive(int page=1)
        {
            if (page < 1)
                return RedirectToActionPermanent("Archive", new {page = 1});

            IQueryable<Content> calendars =
                _contentService.GetAllContentsByTypes(new[] {ContentType.Calendars}).Where(c => c.IsPublished);
            calendars = calendars.OrderBy(c => c.DisplayOrder).ThenByDescending(c => c.PublishDateTicks);

            IPagination<Content> results;

            if (ControllerContext.IsChildAction)
                results = new LazyPagination<Content>(calendars, page, Constants.RecordPerPartial);
            else
            {
                results = new LazyPagination<Content>(calendars, page, Constants.RecordPerPage);
            }

            if (!results.Any() && page != 1)
                return RedirectToActionPermanent("Archive", new { page = 1 });

            return ViewOrPartialView(results);
        }

        public ActionResult Details(int id, string slug)
        {
            Content calendar = _contentService.GetContentById(id);

            if (calendar == null || !calendar.IsPublished || calendar.Type != ContentType.Calendars)
                return EntityNotFoundView();

            if (slug != null && calendar.Metadata.SeoSlug != slug)
                return NotFoundView();

            calendar.PageViews++;

            try
            {
                _unitOfWork.SaveChanges();
            }
            catch
            {
            }

            Content parent = calendar;
            ICollection<Content> childs = parent.Childs;

            while (childs.Count == 0 && parent.ParentId.HasValue)
            {
                parent = parent.Parent;
                childs = parent.Childs;
            }

            ViewBag.Childs = childs;
            
            return ViewOrPartialView(calendar);
        }


        [ChildActionOnly]
        public ActionResult RelatedContents(int id)
        {
            Content @calendar = _contentService.GetContentById(id);

            if (@calendar == null)
                return null;

            IQueryable<Content> events =
                _contentService.GetAllContentsByTypes(new[] {ContentType.Calendars})
                    .Where(c => c.IsPublished);

            events = events.Where(c => c.CategoryId == @calendar.CategoryId && c.Id != id).Take(10);

            return ViewOrPartialView(events.ToList());
        }
    }
}