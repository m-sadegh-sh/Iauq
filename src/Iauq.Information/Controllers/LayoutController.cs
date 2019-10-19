using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FarsiLibrary.Utils;
using Iauq.Core.Domain;
using Iauq.Data.Services;
using Iauq.Information.Models.Layout;

namespace Iauq.Information.Controllers
{
    public class LayoutController : ControllerBase
    {
        private readonly IContentService _contentService;

        public LayoutController(IContentService contentService)
        {
            _contentService = contentService;
        }

        public PartialViewResult Styles()
        {
            return PartialView("_Styles");
        }

        public PartialViewResult TopBar()
        {
            return PartialView("_TopBar");
        }

        public PartialViewResult NavBar()
        {
            List<Content> menus =
                _contentService.GetAllContentsByTypes(new[] {ContentType.Menu}).OrderBy(c => c.DisplayOrder).Where(
                    p => p.IsPublished && p.Parent == null).ToList();

            return PartialView("_NavBar", menus);
        }

        public PartialViewResult Footer()
        {
            ViewBag.Pages =
                _contentService.GetAllContentsByTypes(new[] {ContentType.Menu}).Where(c => c.IsPublished).OrderBy(
                    c => c.DisplayOrder).ThenByDescending(c => c.PageViews).ThenBy(
                        c => c.Parent == null && c.Childs.Any())
                    .ToList();
            ViewBag.LatestNews =
                _contentService.GetAllContentsByTypes(new[] {ContentType.News}).Where(c => c.IsPublished).OrderBy(
                    c => c.DisplayOrder).OrderByDescending(c => c.PublishDateTicks).Take(5)
                    .ToList
                    ();
            ViewBag.LatestEvents =
                _contentService.GetAllContentsByTypes(new[] {ContentType.Events}).Where(c => c.IsPublished).OrderBy(
                    c => c.DisplayOrder).OrderByDescending(c => c.PublishDateTicks).Take
                    (5).
                    ToList();
            ViewBag.LatestCalendars =
                _contentService.GetAllContentsByTypes(new[] {ContentType.Calendars}).Where(c => c.IsPublished).OrderBy(
                    c => c.DisplayOrder).OrderByDescending(c => c.PublishDateTicks).
                    Take(5).
                    ToList();

            return PartialView("_Footer");
        }

        public PartialViewResult Scripts()
        {
            return PartialView("_Scripts");
        }

        public PartialViewResult CurrentDate()
        {
            var currentDateModel = new CurrentDateModel
                                       {
                                           CurrentDate = PersianDate.Now
                                       };

            return PartialView("_CurrentDate", currentDateModel);
        }
    }
}