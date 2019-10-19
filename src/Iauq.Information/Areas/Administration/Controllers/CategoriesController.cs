using System.Linq;
using System.Web.Mvc;
using Iauq.Core.Domain;
using Iauq.Data;
using Iauq.Data.Services;
using Iauq.Information.App_GlobalResources;
using Iauq.Information.Helpers;
using Iauq.Information.LogProviders;
using MvcContrib.Pagination;

namespace Iauq.Information.Areas.Administration.Controllers
{
    [CustomAuthorize(Roles = "Administrators, Moderators")]
    public class CategoriesController : AdministrationControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILanguageService _languageService;
        private readonly IUnitOfWork _unitOfWork;

        public CategoriesController(IUnitOfWork unitOfWork, ILanguageService languageService,
                                    ICategoryService categoryService)
        {
            _unitOfWork = unitOfWork;
            _languageService = languageService;
            _categoryService = categoryService;
        }

        [HttpGet]
        public ActionResult List(int page = 1)
        {
            if (page < 1)
                return RedirectToActionPermanent("List", new {page = 1});

            var results = new LazyPagination<Category>(_categoryService.GetAllCategories().OrderBy(c => c.Id), page,
                                                       Constants.RecordPerPage);

            if (!results.Any() && page != 1)
                return RedirectToActionPermanent("List", new {page = 1});

            return ViewOrPartialView(results);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var category = new Category();

            ViewBag.Languages = new SelectList(_languageService.GetAllLanguages().ToList(), "Id", "Name");
            ViewBag.Parents = new SelectList(_categoryService.GetAllCategories().ToList(), "Id", "Title");

            return ViewOrPartialView(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Languages =
                    new SelectList(_languageService.GetAllLanguages().ToList(), "Id", "Name", category.LanguageId);

                ViewBag.Parents = new SelectList(_categoryService.GetAllCategories().ToList(), "Id", "Title",
                                                 category.ParentId);

                ModelState.AddModelError("",
                                         ValidationResources.InvalidState);

                return ViewOrPartialView(category);
            }

            _categoryService.SaveCategory(category);

            bool isSaved;

            try
            {
                isSaved = _unitOfWork.SaveChanges() > 0;
            }
            catch
            {
                isSaved = false;
            }

            if(isSaved)
                Logger.SaveLog(new CreateCategoryProvider(category));
            else
            {
                ViewBag.Languages =
                    new SelectList(_languageService.GetAllLanguages().ToList(), "Id", "Name", category.LanguageId);

                ViewBag.Parents = new SelectList(_categoryService.GetAllCategories().ToList(), "Id", "Title",
                                                 category.ParentId);

                ModelState.AddModelError("", ValidationResources.CreationFailure);

                return ViewOrPartialView(category);
            }

            if (IsReferrerValid())
                return Redirect(Request.UrlReferrer.AbsolutePath);

            return RedirectToAction("List",new{page=1});
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            Category category = _categoryService.GetCategoryById(id);

            if (category == null)
                return EntityNotFoundView();

            ViewBag.Languages = new SelectList(_languageService.GetAllLanguages().ToList(), "Id", "Name",
                                               category.LanguageId);

            ViewBag.Parents =
                new SelectList(_categoryService.GetAllCategories().Where(c => c.Id != category.Id).ToList(),
                               "Id", "Title",
                               category.ParentId);

            return ViewOrPartialView(category);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int id)
        {
            Category dbCategory = _categoryService.GetCategoryById(id);

            if (dbCategory == null)
                return EntityNotFoundView();

            TryUpdateModel(dbCategory, new[] {"Title", "Metadata", "LanguageId", "ParentId", "DisplayOrder"});

            if (!TryValidateModel(dbCategory))
            {
                ViewBag.Languages =
                    new SelectList(_languageService.GetAllLanguages().ToList(), "Id", "Name", dbCategory.LanguageId);

                ViewBag.Parents =
                    new SelectList(_categoryService.GetAllCategories().Where(c => c.Id != dbCategory.Id).ToList(), "Id",
                                   "Title",
                                   dbCategory.ParentId);

                ModelState.AddModelError("",
                                         ValidationResources.InvalidState);

                return ViewOrPartialView(dbCategory);
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

            if(isSaved)
                Logger.SaveLog(new UpdateCategoryProvider(dbCategory));
            else
            {
                ViewBag.Languages =
                    new SelectList(_languageService.GetAllLanguages().ToList(), "Id", "Name", dbCategory.LanguageId);

                ViewBag.Parents =
                    new SelectList(_categoryService.GetAllCategories().Where(c => c.Id != dbCategory.Id).ToList(), "Id",
                                   "Title",
                                   dbCategory.ParentId);

                ModelState.AddModelError("", ValidationResources.UpdateFailure);

                return ViewOrPartialView(dbCategory);
            }

            if (IsReferrerValid())
                return Redirect(Request.UrlReferrer.AbsolutePath);

            return RedirectToAction("List",new{page=1});
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            Category dbCategory = _categoryService.GetCategoryById(id);

            if (dbCategory == null)
                return EntityNotFoundView();

            _categoryService.DeleteCategory(dbCategory);

            bool isSaved;

            try
            {
                isSaved = _unitOfWork.SaveChanges() > 0;
            }
            catch
            {
                isSaved = false;
            }

            if(isSaved)
                Logger.SaveLog(new DeleteCategoryProvider(dbCategory.Id));
            else
                TempData["Error"] = ValidationResources.DeleteFailure;

            if (IsReferrerValid())
                return Redirect(Request.UrlReferrer.AbsolutePath);

            return RedirectToAction("List",new{page=1});
        }
    }
}