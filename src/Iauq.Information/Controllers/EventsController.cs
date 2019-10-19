using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Iauq.Core.Domain;
using Iauq.Core.Utilities;
using Iauq.Data;
using Iauq.Data.Services;
using Iauq.Information.App_GlobalResources;
using Iauq.Information.Helpers;
using Iauq.Information.LogProviders;
using MvcContrib.Pagination;
using StructureMap;

namespace Iauq.Information.Controllers
{
    public class EventsController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly IContentService _contentService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHelper _webHelper;

        public EventsController(IContentService contentService, ICommentService commentService, IUnitOfWork unitOfWork,
                                IWebHelper webHelper)
        {
            _contentService = contentService;
            _commentService = commentService;
            _unitOfWork = unitOfWork;
            _webHelper = webHelper;
        }

        public ActionResult Archive(int page = 1)
        {
            if (page < 1)
                return RedirectToActionPermanent("Archive", new {page = 1});

            IQueryable<Content> events =
                _contentService.GetAllContentsByTypes(new[] {ContentType.Events}).Where(c => c.IsPublished);
            events = events.OrderBy(c => c.PublishDateTicks);

            IPagination<Content> results;

            if (ControllerContext.IsChildAction)
                results = new LazyPagination<Content>(events, page, Constants.RecordPerPartial);
            else
            {
                results = new LazyPagination<Content>(events, page, Constants.RecordPerPage);
            }

            if (!results.Any() && page != 1)
                return RedirectToActionPermanent("Archive", new {page = 1});

            return ViewOrPartialView(results);
        }

        public ActionResult Details(int id, string slug)
        {
            Content @event = _contentService.GetContentById(id);

            if (@event == null || !@event.IsPublished || @event.Type != ContentType.Events)
                return EntityNotFoundView();

            if (slug != null && @event.Metadata.SeoSlug != slug)
                return NotFoundView();

            @event.PageViews++;

            try
            {
                _unitOfWork.SaveChanges();
            }
            catch
            {
            }

            Content parent = @event;
            ICollection<Content> childs = parent.Childs;

            while (childs.Count == 0 && parent.ParentId.HasValue)
            {
                parent = parent.Parent;
                childs = parent.Childs;
            }

            ViewBag.Childs = childs;

            return ViewOrPartialView(@event);
        }


        [HttpPost]
        public ActionResult Details(int id, string slug, string title, string body)
        {
            Content @event = _contentService.GetContentById(id);

            if (@event == null || !@event.IsPublished || @event.Type != ContentType.Events)
                return NotFoundView();

            if (slug != null && @event.Metadata.SeoSlug != slug)
                return NotFoundView();

            return ViewOrPartialView(@event);

            var comment = new Comment
                              {
                                  Title = title,
                                  Body = body,
                                  CommentDateTicks = DateTime.Now.Ticks,
                                  CommentorIp = ObjectFactory.GetInstance<IWebHelper>().GetIpAddress(),
                                  Owner = @event
                              };

            if (Request.IsAuthenticated)
                comment.Commentor = _webHelper.GetCurrentUser(ControllerContext.HttpContext);

            TryUpdateModel(comment, new[] {"Title", "Body"});

            if (!TryValidateModel(comment))
            {
                ModelState.AddModelError("",
                                         ValidationResources.InvalidState);

                ViewBag.Comment = comment;

                return ViewOrPartialView(@event);
            }

            comment.IsApproved = _webHelper.IsInRole(comment.Commentor, "Administrators", "Moderatos");
            _commentService.SaveComment(comment);

            bool isSaved;

            try
            {
                isSaved = _unitOfWork.SaveChanges() > 0;
            }
            catch
            {
                isSaved = false;
            }

            if (isSaved)
                Logger.SaveLog(new CreateCommentProvider(comment));
            else
            {
                ModelState.AddModelError("", ValidationResources.CommentSubmissionFailure);

                ViewBag.Comment = comment;

                return ViewOrPartialView(@event);
            }

            TempData["CommentSubmitted"] = true;

            return RedirectToAction("Details", new {@event.Id, slug = @event.Metadata.SeoSlug});
        }

        [ChildActionOnly]
        public ActionResult RelatedContents(int id)
        {
            Content @event = _contentService.GetContentById(id);

            if (@event == null)
                return null;

            IQueryable<Content> events =
                _contentService.GetAllContentsByTypes(new[] {ContentType.Events})
                    .Where(c => c.IsPublished);

            events = events.Where(c => c.CategoryId == @event.CategoryId && c.Id != id).Take(10);

            return ViewOrPartialView(events.ToList());
        }
    }
}