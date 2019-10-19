using System.Linq;
using System.Web.Mvc;
using Iauq.Core.Domain;
using Iauq.Data;
using Iauq.Data.Services;
using Iauq.Information.App_GlobalResources;
using Iauq.Information.Helpers;
using MvcContrib.Pagination;

namespace Iauq.Information.Areas.Administration.Controllers
{
    public class LogsController : AdministrationControllerBase
    {
        private readonly ILogService _logService;
        private readonly IUnitOfWork _unitOfWork;

        public LogsController(IUnitOfWork unitOfWork, ILogService logService)
        {
            _unitOfWork = unitOfWork;
            _logService = logService;
        }

        [HttpGet]
        public ActionResult List(string logLevel = null, int page = 1)
        {
            IPagination<Log> results;

            if (logLevel == null)
            {
                if (page < 1)
                    return RedirectToActionPermanent("List", new {page = 1});

                IQueryable<Log> query = _logService.GetAllLogs().OrderByDescending(l => l.LogDate);
                
                results = new LazyPagination<Log>(query, page,
                                                  Constants.RecordPerPage);

                if (!results.Any() && page != 1)
                    return RedirectToActionPermanent("List", new {page = 1});
            }
            else
            {
                LogLevel level;

                if (!ExtractLogLevel(logLevel, out level))
                    return NotFoundView();

                if (page < 1)
                    return RedirectToActionPermanent("List", new {page = 1, logLevel = level});

                IQueryable<Log> query = _logService.GetAllLogsByLevel(new[] {level}).OrderByDescending(l => l.LogDate);
                
                results =
                    new LazyPagination<Log>(query,
                                            page,
                                            Constants.RecordPerPage);

                if (!results.Any() && page != 1)
                    return RedirectToActionPermanent("List", new {page = 1, logLevel = level});

                ViewBag.LogLevel = level;
            }

            return ViewOrPartialView(results);
        }

        private bool ExtractLogLevel(string logLevel, out LogLevel level)
        {
            switch ((logLevel ?? string.Empty).ToLowerInvariant())
            {
                case "wrong-credentials":
                    level = LogLevel.WrongCredentials;
                    return true;
                case "login":
                    level = LogLevel.Login;
                    return true;
                case "change-password":
                    level = LogLevel.ChangePassword;
                    return true;
                case "logout":
                    level = LogLevel.Logout;
                    return true;
                case "create":
                    level = LogLevel.Create;
                    return true;
                case "update":
                    level = LogLevel.Update;
                    return true;
                case "delete":
                    level = LogLevel.Delete;
                    return true;
                case "exception":
                    level = LogLevel.Exception;
                    return true;
            }

            level = LogLevel.Exception;
            return false;
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            Log log = _logService.GetLogById(id);

            if (log == null)
                return EntityNotFoundView();

            return ViewOrPartialView(log);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            Log dbLog = _logService.GetLogById(id);

            if (dbLog == null)
                return EntityNotFoundView();

            _logService.DeleteLog(dbLog);

            bool isSaved;

            try
            {
                isSaved = _unitOfWork.SaveChanges() > 0;
            }
            catch
            {
                isSaved = false;
            }

            if (!isSaved)
                TempData["Error"] = ValidationResources.DeleteFailure;

            if (IsReferrerValid())
                return Redirect(Request.UrlReferrer.AbsolutePath);

            return RedirectToAction("List", new {page = 1});
        }
    }
}