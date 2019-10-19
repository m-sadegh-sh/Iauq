using System.Collections.Generic;
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
    public class UsersController : AdministrationControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;

        public UsersController(IUnitOfWork unitOfWork, IRoleService roleService,
                               IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _roleService = roleService;
            _userService = userService;
        }

        [HttpGet]
        public ActionResult List(int page = 1)
        {
            if (page < 1)
                return RedirectToActionPermanent("List", new {page = 1});

            var results = new LazyPagination<User>(_userService.GetAllUsers().OrderBy(c => c.Id), page,
                                                   Constants.RecordPerPage);

            if (!results.Any() && page != 1)
                return RedirectToActionPermanent("List", new {page = 1});

            return ViewOrPartialView(results);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var user = new User();

            ViewBag.Roles = _roleService.GetAllRoles().ToList();

            return ViewOrPartialView(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User user, int[] roleIds)
        {
            if (user.Roles != null)
                user.Roles.Clear();

            if (roleIds != null && roleIds.Any())
            {
                List<Role> roles = _roleService.GetAllRoles().Where(r => roleIds.Any(ri => r.Id == ri)).ToList();

                user.Roles = roles;
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Roles = _roleService.GetAllRoles().ToList();

                ModelState.AddModelError("",
                                         ValidationResources.InvalidState);

                return ViewOrPartialView(user);
            }

            _userService.SaveUser(user);

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
                Logger.SaveLog(new CreateUserProvider(user));
            else
            {
                ViewBag.Roles = _roleService.GetAllRoles().ToList();

                ModelState.AddModelError("", ValidationResources.CreationFailure);

                return ViewOrPartialView(user);
            }

            if (IsReferrerValid())
                return Redirect(Request.UrlReferrer.AbsolutePath);

            return RedirectToAction("List", new {page = 1});
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            User user = _userService.GetUserById(id);

            if (user == null)
                return EntityNotFoundView();

            ViewBag.Roles = _roleService.GetAllRoles().ToList();

            return ViewOrPartialView(user);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int id, int[] roleIds)
        {
            User dbUser = _userService.GetUserById(id);

            if (dbUser == null)
                return EntityNotFoundView();

            dbUser.Roles.Clear();

            if (roleIds != null && roleIds.Any())
            {
                List<Role> roles = _roleService.GetAllRoles().Where(r => roleIds.Any(ri => r.Id == ri)).ToList();


                foreach (Role role in roles)
                {
                    dbUser.Roles.Add(role);
                }
            }

            TryUpdateModel(dbUser, new[] {"UserName", "Email", "Password", "SecurityToken"});

            if (!TryValidateModel(dbUser))
            {
                ViewBag.Roles = _roleService.GetAllRoles().ToList();

                ModelState.AddModelError("",
                                         ValidationResources.InvalidState);

                return ViewOrPartialView(dbUser);
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
                Logger.SaveLog(new UpdateUserProvider(dbUser));
            else
            {
                ViewBag.Roles = _roleService.GetAllRoles().ToList();

                ModelState.AddModelError("", ValidationResources.UpdateFailure);

                return ViewOrPartialView(dbUser);
            }

            if (IsReferrerValid())
                return Redirect(Request.UrlReferrer.AbsolutePath);

            return RedirectToAction("List", new {page = 1});
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            User dbUser = _userService.GetUserById(id);

            if (dbUser == null)
                return EntityNotFoundView();

            _userService.DeleteUser(dbUser);

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
                Logger.SaveLog(new DeleteUserProvider(dbUser.UserName));
            else
                TempData["Error"] = ValidationResources.DeleteFailure;

            if (IsReferrerValid())
                return Redirect(Request.UrlReferrer.AbsolutePath);

            return RedirectToAction("List", new {page = 1});
        }
    }
}