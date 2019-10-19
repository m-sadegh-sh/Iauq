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
    public class TemplatesController : AdministrationControllerBase
    {
        private readonly ITemplateService _templateService;
        private readonly IUnitOfWork _unitOfWork;

        public TemplatesController(IUnitOfWork unitOfWork, ITemplateService templateService)
        {
            _unitOfWork = unitOfWork;
            _templateService = templateService;
        }

        [HttpGet]
        public ActionResult List(int page = 1)
        {
            if (page < 1)
                return RedirectToActionPermanent("List", new {page = 1});

            var results = new LazyPagination<Template>(_templateService.GetAllTemplates().OrderBy(c => c.Id), page,
                                                       Constants.RecordPerPage);

            if (!results.Any() && page != 1)
                return RedirectToActionPermanent("List", new {page = 1});

            return ViewOrPartialView(results);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var template = new Template();

            return ViewOrPartialView(template);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Template template)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("",
                                         ValidationResources.InvalidState);

                return ViewOrPartialView(template);
            }

            _templateService.SaveTemplate(template);

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
                Logger.SaveLog(new CreateTemplateProvider(template));
            else
            {
                ModelState.AddModelError("", ValidationResources.CreationFailure);

                return ViewOrPartialView(template);
            }

            if (IsReferrerValid())
                return Redirect(Request.UrlReferrer.AbsolutePath);

            return RedirectToAction("List", new {page = 1});
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            Template template = _templateService.GetTemplateById(id);

            if (template == null)
                return EntityNotFoundView();

            return ViewOrPartialView(template);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int id)
        {
            Template dbTemplate = _templateService.GetTemplateById(id);

            if (dbTemplate == null)
                return EntityNotFoundView();

            TryUpdateModel(dbTemplate, new[] {"Title", "Description", "Body"});

            if (!TryValidateModel(dbTemplate))
            {
                ModelState.AddModelError("",
                                         ValidationResources.InvalidState);

                return ViewOrPartialView(dbTemplate);
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
                Logger.SaveLog(new UpdateTemplateProvider(dbTemplate));
            else
            {
                ModelState.AddModelError("", ValidationResources.UpdateFailure);

                return ViewOrPartialView(dbTemplate);
            }

            if (IsReferrerValid())
                return Redirect(Request.UrlReferrer.AbsolutePath);

            return RedirectToAction("List", new {page = 1});
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            Template dbTemplate = _templateService.GetTemplateById(id);

            if (dbTemplate == null)
                return EntityNotFoundView();

            _templateService.DeleteTemplate(dbTemplate);

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
                Logger.SaveLog(new DeleteTemplateProvider(dbTemplate.Id));
            else
                TempData["Error"] = ValidationResources.DeleteFailure;

            if (IsReferrerValid())
                return Redirect(Request.UrlReferrer.AbsolutePath);

            return RedirectToAction("List", new {page = 1});
        }
    }
}