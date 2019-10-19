using System.Linq;
using System.Web.Mvc;
using Iauq.Core.Domain;
using Iauq.Data.Services;
using Iauq.Information.App_GlobalResources;
using Iauq.Information.Areas.Administration.Models.Administration;
using Iauq.Information.Helpers;
using Iauq.Information.LogProviders;

namespace Iauq.Information.Areas.Administration.Controllers
{
    [CustomAuthorize]
    public class AdministrationController : AdministrationControllerBase
    {
        private readonly ITemplateService _templateService;
        private readonly IUserService _userService;

        public AdministrationController(IUserService userService, ITemplateService templateService)
        {
            _userService = userService;
            _templateService = templateService;
        }

        public ActionResult Templates()
        {
            if (!Request.IsAjaxRequest())
                return AccessDeniedView();

            IQueryable<Template> results = _templateService.GetAllTemplates();

            return ViewOrPartialView(results);
        }

        public ActionResult Default()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View(new ChangePasswordModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordModel changePasswordModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", ValidationResources.InvalidState);
                return View(changePasswordModel);
            }

            bool isValid = _userService.ValidateUser(User.Identity.Name, changePasswordModel.OldPassword,
                                                     changePasswordModel.SecurityToken);

            if (isValid)
            {
                bool isSaved = true;

                try
                {
                    _userService.ChangePassword(User.Identity.Name, changePasswordModel.NewPassword);
                }
                catch
                {
                    isSaved = false;
                }

                if (isSaved)
                    Logger.SaveLog(new UserChangePasswordProvider(changePasswordModel));
                else
                {
                    ModelState.AddModelError("", ValidationResources.UpdateFailure);
                    return ViewOrPartialView(changePasswordModel);
                }

                return RedirectToAction("Default");
            }

            ModelState.AddModelError("", ValidationResources.IncorrectOldPassword);
            return View(changePasswordModel);
        }
    }
}