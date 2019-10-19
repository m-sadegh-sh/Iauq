using System.Collections.Generic;
using System.IO;
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
    [CustomAuthorize(Roles = "Administrators, Moderators")]
    public class CarouselsController : AdministrationControllerBase
    {
        private readonly ICarouselService _carouselService;
        private readonly IUnitOfWork _unitOfWork;

        public CarouselsController(IUnitOfWork unitOfWork, ICarouselService carouselService)
        {
            _unitOfWork = unitOfWork;
            _carouselService = carouselService;
        }

        [HttpGet]
        public ActionResult List(int page = 1)
        {
            if (page < 1)
                return RedirectToActionPermanent("List", new {page = 1});

            var results = new LazyPagination<Carousel>(_carouselService.GetAllCarousels().OrderBy(c => c.Id), page,
                                                       Constants.RecordPerPage);

            if (!results.Any() && page != 1)
                return RedirectToActionPermanent("List", new {page = 1});

            return ViewOrPartialView(results);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var carousel = new Carousel();

            return ViewOrPartialView(carousel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Carousel carousel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("",
                                         ValidationResources.InvalidState);

                return ViewOrPartialView(carousel);
            }

            if (carousel.Slide == null)
            {
                ModelState.AddModelError("Slide",
                                         ValidationResources.SlideImageRequired);

                return ViewOrPartialView(carousel);
            }

            if (carousel.Slide.ContentLength > 1000000)
            {
                ModelState.AddModelError("Slide",
                                         ValidationResources.SlideImageLength);

                return ViewOrPartialView(carousel);
            }

            if (!UploadUtilities.IsValidImageBinary(carousel.Slide.InputStream))
            {
                ModelState.AddModelError("Slide",
                                         ValidationResources.SlideImageInvalidBinary);

                return ViewOrPartialView(carousel);
            }

            _carouselService.SaveCarousel(carousel);

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
                Logger.SaveLog(new CreateCarouselProvider(carousel));
            else
            {
                ModelState.AddModelError("", ValidationResources.CreationFailure);

                return ViewOrPartialView(carousel);
            }

            UploadUtilities.TryToSaveImage(carousel.Slide, Constants.CarouselsUrl, carousel.Id.ToString());

            if (IsReferrerValid())
                return Redirect(Request.UrlReferrer.AbsolutePath);

            return RedirectToAction("List", new {page = 1});
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            Carousel carousel = _carouselService.GetCarouselById(id);

            if (carousel == null)
                return EntityNotFoundView();

            return ViewOrPartialView(carousel);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int id)
        {
            Carousel dbCarousel = _carouselService.GetCarouselById(id);

            if (dbCarousel == null)
                return EntityNotFoundView();

            TryUpdateModel(dbCarousel, new[] {"Title", "Description", "LinkUrl", "Slide", "DisplayOrder"});

            if (!TryValidateModel(dbCarousel))
            {
                ModelState.AddModelError("",
                                         ValidationResources.InvalidState);

                return ViewOrPartialView(dbCarousel);
            }

            if (dbCarousel.Slide != null && dbCarousel.Slide.ContentLength > 1000000)
            {
                ModelState.AddModelError("Slide",
                                         ValidationResources.SlideImageLength);

                return ViewOrPartialView(dbCarousel);
            }

            bool isSaved = true;

            try
            {
                isSaved = _unitOfWork.SaveChanges() > 0;
            }
            catch
            {
                isSaved = false;
            }

            if (isSaved)
                Logger.SaveLog(new UpdateCarouselProvider(dbCarousel));
            else
            {
                ModelState.AddModelError("", ValidationResources.UpdateFailure);

                return ViewOrPartialView(dbCarousel);
            }

            if (dbCarousel.Slide != null)
            {
                UploadUtilities.TryToSaveImage(dbCarousel.Slide, Constants.CarouselsUrl, dbCarousel.Id.ToString());
            }

            if (IsReferrerValid())
                return Redirect(Request.UrlReferrer.AbsolutePath);

            return RedirectToAction("List", new {page = 1});
        }


        [HttpGet]
        public ActionResult Delete(int id)
        {
            Carousel dbCarousel = _carouselService.GetCarouselById(id);

            if (dbCarousel == null)
                return EntityNotFoundView();

            _carouselService.DeleteCarousel(dbCarousel);

            bool isSaved = true;

            try
            {
                isSaved = _unitOfWork.SaveChanges() > 0;
            }
            catch
            {
                isSaved = false;
            }

            if (isSaved)
            {
                Logger.SaveLog(new DeleteCarouselProvider(dbCarousel.Id));

                string directory = Server.MapPath(Constants.CarouselsUrl);

                if (Directory.Exists(directory))
                {
                    List<string> filesToDelete = Directory.GetFiles(directory, dbCarousel.Id + ".*").ToList();
                    filesToDelete.AddRange(Directory.GetFiles(directory, dbCarousel.Id + "-thumb.*").ToList());

                    try
                    {
                        foreach (string filePath in filesToDelete)
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }
                    catch
                    {
                    }
                }
            }
            else
                TempData["Error"] = ValidationResources.DeleteFailure;

            if (IsReferrerValid())
                return Redirect(Request.UrlReferrer.AbsolutePath);

            return RedirectToAction("List", new {page = 1});
        }
    }
}