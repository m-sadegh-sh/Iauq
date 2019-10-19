using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
    public class ContentsController : AdministrationControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IContentService _contentService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHelper _webHelper;

        public ContentsController(IUnitOfWork unitOfWork, IContentService contentService,
                                  ICategoryService categoryService, IWebHelper webHelper)
        {
            _unitOfWork = unitOfWork;
            _contentService = contentService;
            _categoryService = categoryService;
            _webHelper = webHelper;
        }


        [HttpGet]
        public ActionResult List(string contentType = null, int page = 1, int recordPerPage = Constants.RecordPerPage)
        {
            IPagination<Content> results;

            if (contentType == null)
            {
                if (page < 1)
                    return RedirectToActionPermanent("List", new {page = 1});

                IQueryable<Content> query = _contentService.GetAllContents();

                ExcludeNotRealatedRecords(ref query);

                query = query.OrderBy(c => c.Id).AsQueryable();

                results = new LazyPagination<Content>(query, page,
                                                      recordPerPage);

                if (!results.Any() && page != 1)
                    return RedirectToActionPermanent("List", new {page = 1});
            }
            else
            {
                ContentType type;

                if (!ExtractContentType(contentType, out type))
                    return NotFoundView();

                if (page < 1)
                    return RedirectToActionPermanent("List", new {page = 1, contentType = type});

                IQueryable<Content> query = _contentService.GetAllContentsByTypes(new[] {type});

                ExcludeNotRealatedRecords(ref query);

                query = query.OrderBy(c => c.Id).AsQueryable();

                results =
                    new LazyPagination<Content>(query,
                                                page,
                                                recordPerPage);

                if (!results.Any() && page != 1)
                    return RedirectToActionPermanent("List", new {page = 1, contentType = type});

                ViewBag.ContentType = type;
            }

            ViewBag.RecordPerPage = recordPerPage;

            return ViewOrPartialView(results);
        }

        private bool UserIsUnlimited()
        {
            User user = _webHelper.GetCurrentUser(ControllerContext.HttpContext);

            if (_webHelper.IsInRole(user, "Administrators") || _webHelper.IsInRole(user, "Moderators"))
                return true;

            return false;
        }

        private void ExcludeNotRealatedRecords(ref IQueryable<Content> query)
        {
            if (UserIsUnlimited())
                return;

            List<int> userCategories = GetUserCategories().Select(c => c.Id).ToList();

            query = from contents in query
                    where
                        userCategories.Contains(contents.CategoryId ?? -1) &&
                        contents.TypeInt != (short) ContentType.Menu
                    select contents;
        }

        private bool UserIsAllowedToCrud(Content content)
        {
            if (UserIsUnlimited())
                return true;

            if (content.TypeInt == (short) ContentType.Menu)
                return false;

            if (content.Id == 0)
                return true;

            List<int> userCategories = GetUserCategories().Select(c => c.Id).ToList();

            return userCategories.Contains(content.CategoryId ?? -1);
        }

        [HttpGet]
        public ActionResult Create(string contentType)
        {
            var content = new Content();

            ContentType type;

            if (!ExtractContentType(contentType, out type))
                return NotFoundView();

            ViewBag.Type = type;
            content.Type = type;

            ViewBag.UserIsUnlimited = UserIsUnlimited();

            if (ViewBag.UserIsUnlimited)
            {
                ViewBag.Categories =
                    _categoryService.GetAllCategories().OrderBy(c => c.ParentId.HasValue ? c.Parent.Title : c.Title).
                        ToList();

                ViewBag.Parents =
                    _contentService.GetAllContentsByTypes(new[] {ContentType.Menu, ContentType.Pages, ContentType.Links})
                        .OrderBy(c => c.ParentId.HasValue ? c.Parent.Title : c.Title).ToList();
            }
            else
            {
                ViewBag.Categories = GetUserCategories();

                ViewBag.Parents = GetParentsBasedOnUserCategories();
            }

            if (!UserIsAllowedToCrud(content))
                return AccessDeniedView();

            return ViewOrPartialView(content);
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

        private bool ExtractContentType(string contentType, out ContentType type)
        {
            switch ((contentType ?? string.Empty).ToLowerInvariant())
            {
                case "news":
                    type = ContentType.News;
                    return true;
                case "event":
                    type = ContentType.Events;
                    return true;
                case "calendar":
                    type = ContentType.Calendars;
                    return true;
                case "page":
                    type = ContentType.Pages;
                    return true;
                case "link":
                    type = ContentType.Links;
                    return true;
                case "menu":
                    type = ContentType.Menu;
                    return true;
            }

            type = ContentType.News;
            return false;
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string contentType, Content content)
        {
            ContentType type;

            if (!ExtractContentType(contentType, out type))
                return NotFoundView();

            ViewBag.Type = type;

            ViewBag.UserIsUnlimited = UserIsUnlimited();

            if (!ModelState.IsValid)
            {
                if (ViewBag.UserIsUnlimited)
                {
                    ViewBag.Categories =
                        _categoryService.GetAllCategories().OrderBy(c => c.ParentId.HasValue ? c.Parent.Title : c.Title)
                            .ToList();

                    ViewBag.Parents =
                        _contentService.GetAllContentsByTypes(new[]
                                                                  {
                                                                      ContentType.Menu, ContentType.Pages,
                                                                      ContentType.Links
                                                                  }).OrderBy(
                                                                      c =>
                                                                      c.ParentId.HasValue ? c.Parent.Title : c.Title).
                            ToList();
                }
                else
                {
                    ViewBag.Categories = GetUserCategories();

                    ViewBag.Parents = GetParentsBasedOnUserCategories();
                }

                ModelState.AddModelError("",
                                         ValidationResources.InvalidState);

                return ViewOrPartialView(content);
            }

            content.Type = type;
            content.PublishDateTicks = DateTime.Now.Ticks;
            content.Author = _webHelper.GetCurrentUser(ControllerContext.HttpContext);
            content.IsPublished = true;

            if (content.Metadata == null)
                content.Metadata = new SeoMetadata();

            if (!string.IsNullOrEmpty(content.Metadata.SeoSlug))
                content.Metadata.SeoSlug = ValidateSlug(content.Metadata.SeoSlug);
            else
            {
                content.Metadata.SeoSlug = ValidateSlug(content.Title);
            }

            if (!string.IsNullOrEmpty(content.Tags))
                content.Tags = content.Tags.Trim(new[] {',', ';', '،'}).Trim();

            if (!UserIsAllowedToCrud(content))
                return AccessDeniedView();

            _contentService.SaveContent(content);

            bool isSaved;

            try
            {
                if (content.Metadata == null)
                    content.Metadata = new SeoMetadata();
                isSaved = _unitOfWork.SaveChanges() > 0;
            }
            catch
            {
                isSaved = false;
            }

            if (isSaved)
                Logger.SaveLog(new CreateContentProvider(content));
            else
            {
                if (ViewBag.UserIsUnlimited)
                {
                    ViewBag.Categories =
                        _categoryService.GetAllCategories().OrderBy(c => c.ParentId.HasValue ? c.Parent.Title : c.Title)
                            .ToList();

                    ViewBag.Parents =
                        _contentService.GetAllContentsByTypes(new[]
                                                                  {
                                                                      ContentType.Menu, ContentType.Pages,
                                                                      ContentType.Links,
                                                                  }).OrderBy(
                                                                      c =>
                                                                      c.ParentId.HasValue ? c.Parent.Title : c.Title).
                            ToList();
                }
                else
                {
                    ViewBag.Categories = GetUserCategories();

                    ViewBag.Parents = GetParentsBasedOnUserCategories();
                }

                ModelState.AddModelError("", ValidationResources.CreationFailure);

                return ViewOrPartialView(content);
            }

            if (IsReferrerValid())
                return Redirect(Request.UrlReferrer.AbsolutePath);

            return RedirectToAction("List", new {page = 1});
        }

        private string ValidateSlug(string unvalidatedSlug)
        {
            if (string.IsNullOrWhiteSpace(unvalidatedSlug))
                return "";

            unvalidatedSlug = Regex.Replace(unvalidatedSlug, @"[^A-Za-z0-9آا-ی]+", "-");

            unvalidatedSlug = Regex.Replace(unvalidatedSlug, "--", "-");

            unvalidatedSlug = unvalidatedSlug.Trim('-');

            return unvalidatedSlug;
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            Content content = _contentService.GetContentById(id);

            if (content == null)
                return EntityNotFoundView();

            ViewBag.Type = content.Type;

            ViewBag.UserIsUnlimited = UserIsUnlimited();

            if (ViewBag.UserIsUnlimited)
            {
                ViewBag.Categories =
                    _categoryService.GetAllCategories().OrderBy(c => c.ParentId.HasValue ? c.Parent.Title : c.Title).
                        ToList();

                ViewBag.Parents =
                    _contentService.GetAllContentsByTypes(new[] {ContentType.Menu, ContentType.Pages, ContentType.Links})
                        .Where(c => c.Id != content.Id).OrderBy(
                            c => c.ParentId.HasValue ? c.Parent.Title : c.Title).ToList();
            }
            else
            {
                ViewBag.Categories = GetUserCategories();

                ViewBag.Parents = GetParentsBasedOnUserCategories(content.Id);
            }

            if (!UserIsAllowedToCrud(content))
                return AccessDeniedView();

            return ViewOrPartialView(content);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [ActionName("Edit")]
        public ActionResult EditPost(int id)
        {
            Content dbContent = _contentService.GetContentById(id);

            if (dbContent == null)
                return EntityNotFoundView();

            TryUpdateModel(dbContent, new[]
                                          {
                                              "IsHot", "ShowNestedContents", "TypeInt", "PublishDate", "Title",
                                              "Abstract", "Body", "Tags",
                                              "Metadata", "ParentId", "DisplayOrder"
                                          });

            ViewBag.UserIsUnlimited = UserIsUnlimited();

            if (!TryValidateModel(dbContent))
            {
                ViewBag.Type = dbContent.Type;

                if (ViewBag.UserIsUnlimited)
                {
                    ViewBag.Categories =
                        _categoryService.GetAllCategories().OrderBy(c => c.ParentId.HasValue ? c.Parent.Title : c.Title)
                            .ToList();

                    ViewBag.Parents =
                        _contentService.GetAllContentsByTypes(new[]
                                                                  {
                                                                      ContentType.Menu, ContentType.Pages,
                                                                      ContentType.Links,
                                                                  }).Where(c => c.Id != dbContent.Id).OrderBy(
                                                                      c =>
                                                                      c.ParentId.HasValue ? c.Parent.Title : c.Title).
                            ToList();
                }
                else
                {
                    ViewBag.Categories = GetUserCategories();

                    ViewBag.Parents = GetParentsBasedOnUserCategories(dbContent.Id);
                }

                ModelState.AddModelError("",
                                         ValidationResources.InvalidState);

                return ViewOrPartialView(dbContent);
            }

            if (!string.IsNullOrEmpty(dbContent.Tags))
                dbContent.Tags = dbContent.Tags.Trim(new[] {',', ';', '،'}).Trim();

            if (!string.IsNullOrEmpty(dbContent.Metadata.SeoSlug))
                dbContent.Metadata.SeoSlug = ValidateSlug(dbContent.Metadata.SeoSlug);
            else
            {
                dbContent.Metadata.SeoSlug = ValidateSlug(dbContent.Title);
            }

            if (!UserIsAllowedToCrud(dbContent))
                return AccessDeniedView();

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
                Logger.SaveLog(new UpdateContentProvider(dbContent));
            else
            {
                ViewBag.Type = dbContent.Type;

                if (ViewBag.UserIsUnlimited)
                {
                    ViewBag.Categories =
                        _categoryService.GetAllCategories().OrderBy(c => c.ParentId.HasValue ? c.Parent.Title : c.Title)
                            .ToList();

                    ViewBag.Parents =
                        _contentService.GetAllContentsByTypes(new[]
                                                                  {
                                                                      ContentType.Menu, ContentType.Pages,
                                                                      ContentType.Links,
                                                                  }).Where(c => c.Id != dbContent.Id).OrderBy(
                                                                      c =>
                                                                      c.ParentId.HasValue ? c.Parent.Title : c.Title).
                            ToList();
                }
                else
                {
                    ViewBag.Categories = GetUserCategories();

                    ViewBag.Parents = GetParentsBasedOnUserCategories(dbContent.Id);
                }

                ModelState.AddModelError("", ValidationResources.UpdateFailure);

                return ViewOrPartialView(dbContent);
            }

            if (IsReferrerValid())
                return Redirect(Request.UrlReferrer.AbsolutePath);

            return RedirectToAction("List", new {page = 1});
        }

        private IEnumerable<Content> GetParentsBasedOnUserCategories(int? excludedContentId = null)
        {
            User user = _webHelper.GetCurrentUser(ControllerContext.HttpContext);

            List<Role> roles = user.Roles.ToList();

            List<int> categoryIds = roles.Where(r => r.Category != null).Select(r => r.Category.Id).ToList();

            IQueryable<Content> contents =
                _contentService.GetAllContentsByTypes(new[] {ContentType.Menu, ContentType.Pages, ContentType.Links}).
                    Where(c => (categoryIds.Contains(c.CategoryId ?? -1) ||
                                (c.Parent != null &&
                                 categoryIds.Contains(c.Parent.CategoryId ?? -1))));

            if (excludedContentId != null)
                contents = contents.Where(c => c.Id != excludedContentId);

            contents = contents.OrderBy(c => c.ParentId.HasValue ? c.Parent.Title : c.Title);

            return contents.ToList();
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            Content dbContent = _contentService.GetContentById(id);

            if (dbContent == null)
                return EntityNotFoundView();

            if (!UserIsAllowedToCrud(dbContent))
                return AccessDeniedView();

            _contentService.DeleteContent(dbContent);

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
                Logger.SaveLog(new DeleteContentProvider(dbContent.Id));
            else
                TempData["Error"] = ValidationResources.DeleteFailure;

            if (IsReferrerValid())
                return Redirect(Request.UrlReferrer.AbsolutePath);

            return RedirectToAction("List", new {page = 1});
        }

        [HttpGet]
        public ActionResult ChangePublication(int id)
        {
            Content dbContent = _contentService.GetContentById(id);

            if (dbContent == null)
                return EntityNotFoundView();

            dbContent.IsPublished = !dbContent.IsPublished;

            if (!UserIsAllowedToCrud(dbContent))
                return AccessDeniedView();

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
                Logger.SaveLog(new ContentChangePublishProvider(dbContent));
            else
                TempData["Error"] = ValidationResources.UpdateFailure;

            if (IsReferrerValid())
                return Redirect(Request.UrlReferrer.AbsolutePath);

            return RedirectToAction("List", new {page = 1});
        }

        #region Nested type: NameValue

        public class NameValue
        {
            public string Name { get; set; }
            public object Value { get; set; }
        }

        #endregion
    }
}