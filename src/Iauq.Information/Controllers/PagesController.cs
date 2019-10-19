using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Iauq.Core.Domain;
using Iauq.Core.Utilities;
using Iauq.Data;
using Iauq.Data.Services;
using Iauq.Information.App_GlobalResources;
using Iauq.Information.LogProviders;
using StructureMap;

namespace Iauq.Information.Controllers
{
    public class PagesController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly IContentService _contentService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHelper _webHelper;

        public PagesController(IContentService contentService, ICommentService commentService, IUnitOfWork unitOfWork,
                               IWebHelper webHelper)
        {
            _contentService = contentService;
            _commentService = commentService;
            _unitOfWork = unitOfWork;
            _webHelper = webHelper;
        }

        [HttpGet]
        public ActionResult Details(int id, string slug)
        {
            Content page = _contentService.GetContentById(id);

            if (page == null || !page.IsPublished ||
                new[] {ContentType.Pages, ContentType.Menu,}.All(ct => ct != page.Type))
                return NotFoundView();

            if (slug != null && page.Metadata.SeoSlug != slug)
                return NotFoundView();

            page.PageViews++;

            try
            {
                _unitOfWork.SaveChanges();
            }
            catch
            {
            }

            Content parent = page;
            ICollection<Content> childs = parent.Childs;

            while (childs.Count == 0 && parent.ParentId.HasValue)
            {
                parent = parent.Parent;
                childs = parent.Childs;
            }

            ViewBag.Childs = childs;

            return ViewOrPartialView(page);
        }

        [HttpPost]
        public ActionResult Details(int id, string slug, string title, string body)
        {
            Content page = _contentService.GetContentById(id);

            if (page == null || !page.IsPublished ||
                new[] {ContentType.Pages, ContentType.Menu,}.All(ct => ct == page.Type))
                return NotFoundView();

            if (slug != null && page.Metadata.SeoSlug != slug)
                return NotFoundView();

            return ViewOrPartialView(page);

            var comment = new Comment
                              {
                                  Title = title,
                                  Body = body,
                                  CommentDateTicks = DateTime.Now.Ticks,
                                  CommentorIp = ObjectFactory.GetInstance<IWebHelper>().GetIpAddress(),
                                  Owner = page
                              };

            if (Request.IsAuthenticated)
                comment.Commentor = _webHelper.GetCurrentUser(ControllerContext.HttpContext);

            TryUpdateModel(comment, new[] {"Title", "Body"});

            if (!TryValidateModel(comment))
            {
                ModelState.AddModelError("",
                                         ValidationResources.InvalidState);

                ViewBag.Comment = comment;

                return ViewOrPartialView(page);
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

                return ViewOrPartialView(page);
            }

            TempData["CommentSubmitted"] = true;

            return RedirectToAction("Details", new {page.Id, slug = page.Metadata.SeoSlug});
        }

        [ChildActionOnly]
        public ActionResult RelatedContents(int id)
        {
            Content page = _contentService.GetContentById(id);

            if (page == null)
                return null;

            IQueryable<Content> pages =
                _contentService.GetAllContentsByTypes(new[] {ContentType.Pages, ContentType.Menu})
                    .Where(c => c.IsPublished);

            int? parentId = page.ParentId;

            pages = pages.Where(c => c.ParentId == (parentId ?? (int?) id) && c.Id != id);

            pages = pages.Take(10);

            return ViewOrPartialView(pages.ToList());
        }
    }
}