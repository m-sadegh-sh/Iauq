using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Iauq.Core.Domain;
using Iauq.Core.Extensions;
using Iauq.Core.Utilities;
using Iauq.Data;
using Iauq.Data.Services;
using Iauq.Information.App_GlobalResources;
using Iauq.Information.Areas.Administration.Models.Files;
using Iauq.Information.Helpers;
using Iauq.Information.LogProviders;
using MvcContrib.Pagination;
using File = Iauq.Core.Domain.File;

namespace Iauq.Information.Areas.Administration.Controllers
{
    [CustomAuthorize(Roles = "Administrators, Moderators, Editors")]
    public class FilesController : AdministrationControllerBase
    {
        private readonly IFileService _fileService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHelper _webHelper;

        public FilesController(IUnitOfWork unitOfWork, IFileService fileService, IWebHelper webHelper)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _webHelper = webHelper;
        }

        [HttpGet]
        public ActionResult List(int? parentId = null, int page = 1, int recordPerPage = Constants.RecordPerPage)
        {
            IPagination<File> results;

            if (parentId == null)
            {
                if (page < 1)
                    return RedirectToActionPermanent("List", new {page = 1});

                IQueryable<File> query = _fileService.GetAllFiles();

                ExcludeNotRealatedRecords(ref query);

                query = query.OrderBy(c => c.Id).AsQueryable();

                results = new LazyPagination<File>(query, page,
                                                   recordPerPage);

                if (!results.Any() && page != 1)
                    return RedirectToActionPermanent("List", new {page = 1});
            }
            else
            {
                File parent = _fileService.GetFileById(parentId.Value);

                if (parent == null)
                    return NotFoundView();

                if (page < 1)
                    return RedirectToActionPermanent("List", new {page = 1, parentId});

                IQueryable<File> query = _fileService.GetAllFilesByParentId(parent.Id);

                ExcludeNotRealatedRecords(ref query);

                query = query.OrderBy(c => c.Id).AsQueryable();

                results =
                    new LazyPagination<File>(query,
                                             page,
                                             recordPerPage);

                if (!results.Any() && page != 1)
                    return RedirectToActionPermanent("List", new {page = 1, parentId});

                ViewBag.Parent = parent;
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

        private void ExcludeNotRealatedRecords(ref IQueryable<File> query)
        {
            if (UserIsUnlimited())
                return;

            User user = _webHelper.GetCurrentUser(ControllerContext.HttpContext);

            query = from files in query
                    where files.UploaderId == user.Id
                    select files;
        }

        [HttpGet]
        public ActionResult CreateDirectory(int? parentId)
        {
            CreateDbDirectoryModel model;

            if (parentId.HasValue)
            {
                File parent = _fileService.GetFileById(parentId.Value);

                if (parent == null)
                    return NotFoundView();

                if (!UserIsAllowedToCrud(parent))
                    return AccessDeniedView();

                ViewBag.Parent = parent;

                model = new CreateDbDirectoryModel {ParentId = parent.Id};
            }
            else
            {
                model = new CreateDbDirectoryModel();
            }

            ViewBag.AccessModes =
                new SelectList(new Dictionary<short, string>
                                   {
                                       {(short) AccessMode.Any, AccessMode.Any.ToLocalizedString()},
                                       {(short) AccessMode.None, AccessMode.None.ToLocalizedString()},
                                       {
                                           (short) AccessMode.OnlyAuthenticated,
                                           AccessMode.OnlyAuthenticated.ToLocalizedString()
                                           }
                                   }, "Key", "Value");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDirectory(CreateDbDirectoryModel model)
        {
            ViewBag.AccessModes =
                new SelectList(new Dictionary<short, string>
                                   {
                                       {(short) AccessMode.Any, AccessMode.Any.ToLocalizedString()},
                                       {(short) AccessMode.None, AccessMode.None.ToLocalizedString()},
                                       {
                                           (short) AccessMode.OnlyAuthenticated,
                                           AccessMode.OnlyAuthenticated.ToLocalizedString()
                                           }
                                   }, "Key", "Value", model.AccessMode);

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", ValidationResources.InvalidState);

                return View(model);
            }

            if (model.ParentId.HasValue)
            {
                File parent = _fileService.GetFileById(model.ParentId.Value);

                if (parent == null)
                    return NotFoundView();

                if (!UserIsAllowedToCrud(parent))
                    return AccessDeniedView();

                ViewBag.Parent = parent;
            }

            var file = new File
                           {
                               Uploader = _webHelper.GetCurrentUser(HttpContext),
                               AccessModeShort = model.AccessMode,
                               CreateDate = DateTime.UtcNow,
                               Name = model.Name,
                               IsPublished = true,
                               ParentId = model.ParentId
                           };

            _fileService.SaveFile(file);

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
            {
                Logger.SaveLog(new CreateFileProvider(file));
            }
            else
                TempData["Error"] = ValidationResources.CreateDirectoryFailure;

            return RedirectToAction("List", new {file.ParentId, page = 1});
        }

        [HttpGet]
        public ActionResult UploadFile(int? parentId)
        {
            UploadDbFileModel model;

            if (parentId.HasValue)
            {
                File parent = _fileService.GetFileById(parentId.Value);

                if (parent == null)
                    return NotFoundView();

                if (!UserIsAllowedToCrud(parent))
                    return AccessDeniedView();

                ViewBag.Parent = parent;

                model = new UploadDbFileModel {ParentId = parent.Id};
            }
            else
            {
                model = new UploadDbFileModel();
            }

            ViewBag.AccessModes =
                new SelectList(new Dictionary<short, string>
                                   {
                                       {(short) AccessMode.Any, AccessMode.Any.ToLocalizedString()},
                                       {(short) AccessMode.None, AccessMode.None.ToLocalizedString()},
                                       {
                                           (short) AccessMode.OnlyAuthenticated,
                                           AccessMode.OnlyAuthenticated.ToLocalizedString()
                                           }
                                   }, "Key", "Value");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadFile(UploadDbFileModel model)
        {
            ViewBag.AccessModes =
                new SelectList(new Dictionary<short, string>
                                   {
                                       {(short) AccessMode.Any, AccessMode.Any.ToLocalizedString()},
                                       {(short) AccessMode.None, AccessMode.None.ToLocalizedString()},
                                       {
                                           (short) AccessMode.OnlyAuthenticated,
                                           AccessMode.OnlyAuthenticated.ToLocalizedString()
                                           }
                                   }, "Key", "Value", model.AccessMode);

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", ValidationResources.InvalidState);

                return View(model);
            }

            if (model.ParentId.HasValue)
            {
                File parent = _fileService.GetFileById(model.ParentId.Value);

                if (parent == null)
                    return NotFoundView();

                if (!UserIsAllowedToCrud(parent))
                    return AccessDeniedView();

                ViewBag.Parent = parent;
            }

            if (string.IsNullOrWhiteSpace(model.FileName))
                model.FileName = model.PostedFile.FileName;

            const string extensions =
                ".7z|.aiff|.asf|.avi|.bmp|.csv|.doc|.docx|.fla|.flv|.gif|.gz|.gzip|.jpeg|.jpg|.mid|.mov|.mp3|.mp4|.mpc|.mpeg|.mpg|.ods|.odt|.pdf|.png|.ppt|.pxd|.qt|.ram|.rar|.rm|.rmi|.rmvb|.rtf|.sdc|.sitd|.swf|.sxc|.sxw|.tar|.tgz|.tif|.tiff|.txt|.vsd|.wav|.wma|.wmv|.xls|.xml|.zip";

            if (Path.GetExtension(model.FileName) == "")
                model.FileName += Path.GetExtension(model.PostedFile.FileName);

            if (model.ContentLength == 0 ||
                extensions.Split('|').All(e => string.Compare(e, Path.GetExtension(model.FileName), StringComparison.OrdinalIgnoreCase) != 0))
            {
                ModelState.AddModelError("", ValidationResources.SelectedFileIsInvalid);

                return View(model);
            }

            var file = new File
                           {
                               Uploader = _webHelper.GetCurrentUser(HttpContext),
                               AccessModeShort = model.AccessMode,
                               CreateDate = DateTime.UtcNow,
                               Name = model.FileName,
                               ContentType = model.ContentType,
                               Size = model.ContentLength,
                               IsPublished = true,
                               ParentId = model.ParentId
                           };

            _fileService.SaveFile(file);

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
            {
                Logger.SaveLog(new CreateFileProvider(file));

                string targetPath = Server.MapPath(Constants.UploadsUrl);
                UploadUtilities.Save(model.PostedFile, targetPath, file.Guid.ToString());

                return RedirectToAction("List", new {file.ParentId, page = 1});
            }

            ModelState.AddModelError("", ValidationResources.UploadFileFailure);

            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            File file = _fileService.GetFileById(id);

            if (file == null)
                return EntityNotFoundView();

            ViewBag.Parent = file.Parent;

            var model = new DbRenameModel
                            {
                                Id = file.Id,
                                OldName = file.Name,
                                ParentId = file.ParentId,
                                AccessMode = file.AccessModeShort
                            };

            ViewBag.AccessModes =
                new SelectList(new Dictionary<short, string>
                                   {
                                       {(short) AccessMode.Any, AccessMode.Any.ToLocalizedString()},
                                       {(short) AccessMode.None, AccessMode.None.ToLocalizedString()},
                                       {
                                           (short) AccessMode.OnlyAuthenticated,
                                           AccessMode.OnlyAuthenticated.ToLocalizedString()
                                           }
                                   }, "Key", "Value", model.AccessMode);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DbRenameModel model)
        {
            File file = _fileService.GetFileById(model.Id);

            if (file == null)
                return EntityNotFoundView();

            ViewBag.Parent = file.Parent;

            ViewBag.AccessModes =
                new SelectList(new Dictionary<short, string>
                                   {
                                       {(short) AccessMode.Any, AccessMode.Any.ToLocalizedString()},
                                       {(short) AccessMode.None, AccessMode.None.ToLocalizedString()},
                                       {
                                           (short) AccessMode.OnlyAuthenticated,
                                           AccessMode.OnlyAuthenticated.ToLocalizedString()
                                           }
                                   }, "Key", "Value", model.AccessMode);

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", ValidationResources.InvalidState);

                return View(model);
            }

            if (!string.IsNullOrWhiteSpace(model.NewName))
            {
                if (Path.GetExtension(model.NewName) == "")
                    model.NewName += Path.GetExtension(model.OldName);

                file.Name = model.NewName;
            }

            file.AccessModeShort = model.AccessMode;

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
            {
                Logger.SaveLog(new UpdateFileProvider(file));
            }
            else
                TempData["Error"] = ValidationResources.RenameFailure;

            return RedirectToAction("List", new {file.ParentId, page = 1});
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            File file = _fileService.GetFileById(id);

            if (file == null)
                return NotFoundView();

            int? parentId = file.ParentId;

            return DeleteRecursive(file) ?? RedirectToAction("List", new {parentId, page = 1});
        }

        private ActionResult DeleteRecursive(File file)
        {
            if (file == null)
                return null;

            foreach (File child in file.Childs.ToList())
            {
                ActionResult result = DeleteRecursive(child);

                if (result != null)
                    return result;
            }

            if (!UserIsAllowedToCrud(file))
                return AccessDeniedView();

            _fileService.DeleteFile(file);

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
            {
                Logger.SaveLog(new DeleteFileProvider(file.Id));

                if (file.IsFile)
                {
                    string filePath = Server.MapPath(Path.Combine(Constants.UploadsUrl, file.Guid.ToString()));

                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);
                }
            }
            else
                TempData["Error"] = ValidationResources.DeleteDirectoryOrFileFailure;

            return null;
        }

        private bool UserIsAllowedToCrud(File file)
        {
            if (UserIsUnlimited())
                return true;

            if (file.Id == 0)
                return true;

            return file.UploaderId == _webHelper.GetCurrentUser(ControllerContext.HttpContext).Id;
        }

        [HttpGet]
        public ActionResult ChangePublication(int id)
        {
            File dbFile = _fileService.GetFileById(id);

            if (dbFile == null)
                return EntityNotFoundView();

            if (!UserIsAllowedToCrud(dbFile))
                return AccessDeniedView();

            dbFile.IsPublished = !dbFile.IsPublished;

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
                Logger.SaveLog(new FileChangePublishProvider(dbFile));
            else
                TempData["Error"] = ValidationResources.UpdateFailure;

            if (IsReferrerValid())
                return Redirect(Request.UrlReferrer.AbsolutePath);

            return RedirectToAction("List", new {page = 1, dbFile.ParentId});
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