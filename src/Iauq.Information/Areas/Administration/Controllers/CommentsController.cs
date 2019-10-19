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

namespace Iauq.Information.Areas.Administration.Controllers
{
    [CustomAuthorize(Roles = "Administrators, Moderators, Editors")]
    public class CommentsController : AdministrationControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ICommentService _commentService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHelper _webHelper;

        public CommentsController(IUnitOfWork unitOfWork, ICommentService commentService,
                                  ICategoryService categoryService, IWebHelper webHelper)
        {
            _unitOfWork = unitOfWork;
            _commentService = commentService;
            _categoryService = categoryService;
            _webHelper = webHelper;
        }

        [HttpGet]
        public ActionResult List(int? ownerId, int page = 1)
        {
            if (page < 1 || (ownerId.HasValue && ownerId < 1))
            {
                if (ownerId > 0)
                    return RedirectToActionPermanent("List", new {page = 1, ownerId});
                return RedirectToActionPermanent("List", new {page = 1});
            }

            IQueryable<Comment> query;

            if (ownerId.HasValue)
            {
                query = _commentService.GetAllCommentsByOwnerId(ownerId.Value).OrderBy(c => c.Id);
            }
            else
            {
                query = _commentService.GetAllComments().OrderBy(c => c.Id);
            }

            ExcludeNotRealatedRecords(ref query);

            var results =
                new LazyPagination<Comment>(query, page, Constants.RecordPerPage);

            if (!results.Any() && page != 1)
                return RedirectToActionPermanent("List", new {page = 1});

            ViewBag.OwnerId = ownerId;

            return ViewOrPartialView(results);
        }

        private bool UserIsUnlimited()
        {
            User user = _webHelper.GetCurrentUser(ControllerContext.HttpContext);

            if (_webHelper.IsInRole(user, "Administrators") || _webHelper.IsInRole(user, "Moderators"))
                return true;

            return false;
        }

        private IEnumerable<Category> GetUserCategories()
        {
            User user = _webHelper.GetCurrentUser(ControllerContext.HttpContext);

            List<Role> roles = user.Roles.ToList();

            List<int> categoryIds = roles.Where(r => r.Category != null).Select(r => r.Category.Id).ToList();

            return (from categories in _categoryService.GetAllCategories()
                    where categoryIds.Contains(categories.Id) || (categories.Parent != null &&
                                                                  categoryIds.Contains((int) categories.ParentId))
                    orderby categories.ParentId.HasValue ? categories.Parent.Title : categories.Title
                    select categories).ToList();
        }

        private void ExcludeNotRealatedRecords(ref IQueryable<Comment> query)
        {
            if (UserIsUnlimited())
                return;

            List<int> userCategories = GetUserCategories().Select(c => c.Id).ToList();

            query = from comments in query
                    where userCategories.Contains(comments.Owner.CategoryId ?? -1)
                    select comments;
        }

        [HttpGet]
        public ActionResult Create(int id)
        {
            var comment = new Comment();

            return ViewOrPartialView(comment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Comment comment)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("",
                                         ValidationResources.InvalidState);

                return ViewOrPartialView(comment);
            }

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
                ModelState.AddModelError("", ValidationResources.CreationFailure);

                return ViewOrPartialView(comment);
            }

            if (IsReferrerValid())
                return Redirect(Request.UrlReferrer.AbsolutePath);

            return RedirectToAction("List", new {page = 1});
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            Comment comment = _commentService.GetCommentById(id);

            if (comment == null)
                return EntityNotFoundView();

            return ViewOrPartialView(comment);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int id)
        {
            Comment dbComment = _commentService.GetCommentById(id);

            if (dbComment == null)
                return EntityNotFoundView();

            TryUpdateModel(dbComment, new[] {"Title", "Body"});

            if (!TryValidateModel(dbComment))
            {
                ModelState.AddModelError("",
                                         ValidationResources.InvalidState);

                return ViewOrPartialView(dbComment);
            }

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
                Logger.SaveLog(new UpdateCommentProvider(dbComment));
            else
            {
                ModelState.AddModelError("", ValidationResources.UpdateFailure);

                return ViewOrPartialView(dbComment);
            }

            if (IsReferrerValid())
                return Redirect(Request.UrlReferrer.AbsolutePath);

            return RedirectToAction("List", new {page = 1});
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            Comment dbComment = _commentService.GetCommentById(id);

            if (dbComment == null)
                return EntityNotFoundView();

            _commentService.DeleteComment(dbComment);

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
                Logger.SaveLog(new DeleteCommentProvider(dbComment.Id));
            else
                TempData["Error"] = ValidationResources.DeleteFailure;

            if (IsReferrerValid())
                return Redirect(Request.UrlReferrer.AbsolutePath);

            return RedirectToAction("List", new {page = 1});
        }

        [HttpGet]
        public ActionResult ChangeApproval(int id)
        {
            Comment dbComment = _commentService.GetCommentById(id);

            if (dbComment == null)
                return EntityNotFoundView();

            dbComment.IsApproved = !dbComment.IsApproved;

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
                Logger.SaveLog(new CommentChangeApprovalProvider(dbComment));
            else
                TempData["Error"] = ValidationResources.UpdateFailure;

            if (IsReferrerValid())
                return Redirect(Request.UrlReferrer.AbsolutePath);

            return RedirectToAction("List", new {page = 1});
        }
    }
}