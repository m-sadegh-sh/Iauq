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
    public class LanguagesController : AdministrationControllerBase
    {
        private readonly ILanguageService _languageService;
        private readonly IUnitOfWork _unitOfWork;

        public LanguagesController(IUnitOfWork unitOfWork, ILanguageService languageService)
        {
            _unitOfWork = unitOfWork;
            _languageService = languageService;
        }

        [HttpGet]
        public ActionResult List(int page=1)
        {
            if (page < 1)
                return RedirectToActionPermanent("List", new { page = 1 });

            var results = new LazyPagination<Language>(_languageService.GetAllLanguages().OrderBy(c=>c.Id), page,
                                                       Constants.RecordPerPage);

            if (!results.Any() && page != 1)
                return RedirectToActionPermanent("List", new { page = 1 });

            return ViewOrPartialView(results);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var language = new Language();

            return ViewOrPartialView(language);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Language language)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("",
                                         ValidationResources.InvalidState);

                return ViewOrPartialView(language);
            }

            _languageService.SaveLanguage(language);

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
                Logger.SaveLog(new CreateLanguageProvider(language));
            else
            {
                ModelState.AddModelError("", ValidationResources.CreationFailure);

                return ViewOrPartialView(language);
            }

            if (IsReferrerValid())
                return Redirect(Request.UrlReferrer.AbsolutePath);

            return RedirectToAction("List",new{page=1});
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            Language language = _languageService.GetLanguageById(id);

            if (language == null)
                return EntityNotFoundView();

            return ViewOrPartialView(language);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Edit")]
        public ActionResult EditPost(int id)
        {
            Language dbLanguage = _languageService.GetLanguageById(id);

            if (dbLanguage == null)
                return EntityNotFoundView();

            TryUpdateModel(dbLanguage, new[] {"Name", "IsoCode"});

            if (!TryValidateModel(dbLanguage))
            {
                ModelState.AddModelError("",
                                         ValidationResources.InvalidState);

                return ViewOrPartialView(dbLanguage);
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
                Logger.SaveLog(new UpdateLanguageProvider(dbLanguage));
            else
            {
                ModelState.AddModelError("", ValidationResources.UpdateFailure);

                return ViewOrPartialView(dbLanguage);
            }

            if (IsReferrerValid())
                return Redirect(Request.UrlReferrer.AbsolutePath);

            return RedirectToAction("List",new{page=1});
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            Language dbLanguage = _languageService.GetLanguageById(id);

            if (dbLanguage == null)
                return EntityNotFoundView();

            _languageService.DeleteLanguage(dbLanguage);

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
                Logger.SaveLog(new DeleteLanguageProvider(dbLanguage.Id));
            else
                TempData["Error"] = ValidationResources.DeleteFailure;

            if (IsReferrerValid())
                return Redirect(Request.UrlReferrer.AbsolutePath);

            return RedirectToAction("List",new{page=1});
        }
    }
}