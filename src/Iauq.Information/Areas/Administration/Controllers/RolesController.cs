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
    public class RolesController : AdministrationControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IRoleService _roleService;
        private readonly IUnitOfWork _unitOfWork;

        public RolesController(IUnitOfWork unitOfWork, IRoleService roleService, ICategoryService categoryService)
        {
            _unitOfWork = unitOfWork;
            _roleService = roleService;
            _categoryService = categoryService;
        }

        [HttpGet]
        public ActionResult List(int page = 1)
        {
            if (page < 1)
                return RedirectToActionPermanent("List", new {page = 1});

            var results = new LazyPagination<Role>(_roleService.GetAllRoles().OrderBy(c => c.Id), page,
                                                   Constants.RecordPerPage);

            if (!results.Any() && page != 1)
                return RedirectToActionPermanent("List", new {page = 1});

            return ViewOrPartialView(results);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var role = new Role();

            ViewBag.Categories = new SelectList(_categoryService.GetAllCategories().ToList(), "Id", "Title");

            return ViewOrPartialView(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Role role)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(_categoryService.GetAllCategories().ToList(), "Id", "Title",
                                                    role.CategoryId);

                ModelState.AddModelError("",
                                         ValidationResources.InvalidState);

                return ViewOrPartialView(role);
            }

            _roleService.SaveRole(role);

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
                Logger.SaveLog(new CreateRoleProvider(role));
            else
            {
                ViewBag.Categories = new SelectList(_categoryService.GetAllCategories().ToList(), "Id", "Title",
                                                    role.CategoryId);

                ModelState.AddModelError("", ValidationResources.CreationFailure);

                return ViewOrPartialView(role);
            }

            if (IsReferrerValid())
                return Redirect(Request.UrlReferrer.AbsolutePath);

            return RedirectToAction("List", new {page = 1});
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            Role role = _roleService.GetRoleById(id);

            if (role == null)
                return EntityNotFoundView();

            ViewBag.Categories = new SelectList(_categoryService.GetAllCategories().ToList(), "Id", "Title",
                                                role.CategoryId);

            return ViewOrPartialView(role);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int id)
        {
            Role dbRole = _roleService.GetRoleById(id);

            if (dbRole == null)
                return EntityNotFoundView();

            TryUpdateModel(dbRole, new[] {"Name", "CategoryId"});

            if (!TryValidateModel(dbRole))
            {
                ViewBag.Categories = new SelectList(_categoryService.GetAllCategories().ToList(), "Id", "Title",
                                                    dbRole.CategoryId);
                ModelState.AddModelError("",
                                         ValidationResources.InvalidState);

                return ViewOrPartialView(dbRole);
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
                Logger.SaveLog(new UpdateRoleProvider(dbRole));
            else
            {
                ViewBag.Categories = new SelectList(_categoryService.GetAllCategories().ToList(), "Id", "Title",
                                                    dbRole.CategoryId);

                ModelState.AddModelError("", ValidationResources.UpdateFailure);

                return ViewOrPartialView(dbRole);
            }

            if (IsReferrerValid())
                return Redirect(Request.UrlReferrer.AbsolutePath);

            return RedirectToAction("List", new {page = 1});
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            Role dbRole = _roleService.GetRoleById(id);

            if (dbRole == null)
                return EntityNotFoundView();

            _roleService.DeleteRole(dbRole);

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
                Logger.SaveLog(new DeleteRoleProvider(dbRole.Id));
            else
                TempData["Error"] = ValidationResources.DeleteFailure;

            if (IsReferrerValid())
                return Redirect(Request.UrlReferrer.AbsolutePath);

            return RedirectToAction("List", new {page = 1});
        }
    }
}