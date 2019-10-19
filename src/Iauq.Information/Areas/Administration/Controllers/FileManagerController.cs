using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Iauq.Core.Utilities;
using Iauq.Information.App_GlobalResources;
using Iauq.Information.Areas.Administration.Models.FileManager;
using Iauq.Information.Helpers;
using Iauq.Information.LogProviders;

namespace Iauq.Information.Areas.Administration.Controllers
{
    [CustomAuthorize(Roles = "Administrators, Moderators")]
    public class FileManagerController : AdministrationControllerBase
    {
        private readonly IWebHelper _webHelper;

        public FileManagerController(IWebHelper webHelper)
        {
            _webHelper = webHelper;
        }

        public ActionResult List(string currentUrl)
        {
            currentUrl = "/" + currentUrl;

            if (currentUrl.Contains("..") ||
                !currentUrl.StartsWith(Constants.CdnUrl))
                return NotFoundView();

            string currentPath = Server.MapPath(currentUrl);

            if (!Directory.Exists(currentPath))
                return NotFoundView();

            bool processParent = currentUrl != Constants.CdnUrl;

            var model = new FileManagerModel {CurrentEntry = new EntryModel(currentUrl, true, processParent)};

            string[] entries = Directory.GetFileSystemEntries(currentPath);

            foreach (string entry in entries)
            {
                model.Entries.Add(new EntryModel(entry, false, false));
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Rename(string targetUrl)
        {
            targetUrl = "/" + targetUrl;

            if (targetUrl.Contains("..") ||
                !targetUrl.StartsWith(Constants.CdnUrl))
                return NotFoundView();

            string targetPath = Server.MapPath(targetUrl);

            if ((!Directory.Exists(targetPath) && !System.IO.File.Exists(targetPath)))
                return NotFoundView();

            DirectoryInfo parent = Directory.GetParent(targetPath);
            string directoryOrFileName = targetPath.Substring(parent.FullName.Length + 1,
                                                              targetPath.Length - (parent.FullName.Length + 1));


            bool processParent = targetUrl != Constants.CdnUrl;

            return
                View(new RenameModel
                         {
                             TargetUrl = targetUrl,
                             OldName = directoryOrFileName,
                             CurrentEntry = new EntryModel(targetUrl, true, processParent)
                         });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Rename(RenameModel model)
        {
            model.TargetUrl = "/" + model.TargetUrl;

            bool processParent = model.TargetUrl != Constants.CdnUrl;

            model.CurrentEntry = new EntryModel(model.TargetUrl, true, processParent);

            if (model.TargetUrl.Contains("..") ||
                !model.TargetUrl.StartsWith(Constants.CdnUrl))
                return NotFoundView();

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", ValidationResources.InvalidState);

                return View(model);
            }

            if (string.IsNullOrEmpty(model.NewName))
                return RedirectToAction("List",
                                        new
                                            {
                                                currentUrl =
                                            FileManagerModel.EnsureIsDirectoryOrExtractParent(model.TargetUrl)
                                            });


            if (Path.GetExtension(model.NewName) == "")
                model.NewName += Path.GetExtension(model.OldName);

            string targetPath = Server.MapPath(model.TargetUrl);

            DirectoryInfo parent = Directory.GetParent(targetPath);

            string newPath = Path.Combine(parent.FullName, model.NewName);

            if (string.Compare(targetPath, newPath, StringComparison.OrdinalIgnoreCase) == 0)
                return RedirectToAction("List", new {currentUrl = model.TargetUrl});

            if ((!Directory.Exists(targetPath) && !System.IO.File.Exists(targetPath)))
                return NotFoundView();

            bool isSaved;

            try
            {
                if (Directory.Exists(targetPath))
                {
                    Directory.Move(targetPath, newPath);
                }
                else
                {
                    System.IO.File.Move(targetPath, newPath);
                }

                isSaved = true;
            }
            catch
            {
                isSaved = false;
            }

            if (isSaved)
            {
                Logger.SaveLog(new RenameDirectoryOrFileProvider(model));
            }
            else
                TempData["Error"] = ValidationResources.RenameFailure;

            return RedirectToAction("List", new {currentUrl = _webHelper.MapUrl(parent.FullName)});
        }

        [HttpGet]
        public ActionResult CreateDirectory(string targetUrl)
        {
            targetUrl = "/" + targetUrl;

            if (targetUrl.Contains("..") ||
                !targetUrl.StartsWith(Constants.CdnUrl))
                return NotFoundView();

            string targetPath = Server.MapPath(targetUrl);

            if ((!Directory.Exists(targetPath) && !System.IO.File.Exists(targetPath)))
                return NotFoundView();

            bool processParent = targetUrl != Constants.CdnUrl;

            return View(new CreateDirectoryModel
                            {
                                TargetUrl = targetUrl,
                                CurrentEntry = new EntryModel(targetUrl, true, processParent)
                            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDirectory(CreateDirectoryModel model)
        {
            model.TargetUrl = "/" + model.TargetUrl;

            bool processParent = model.TargetUrl != Constants.CdnUrl;

            model.CurrentEntry = new EntryModel(model.TargetUrl, true, processParent);

            if (model.TargetUrl.Contains("..") ||
                !model.TargetUrl.StartsWith(Constants.CdnUrl))
                return NotFoundView();

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", ValidationResources.InvalidState);

                return View(model);
            }

            string targetPath = Server.MapPath(model.TargetUrl);

            string newPath = Path.Combine(targetPath, model.Name);

            if (Directory.Exists(newPath))
            {
                ModelState.AddModelError("", ValidationResources.DuplicateDirectoryName);

                return View(model);
            }

            bool isSaved;

            try
            {
                Directory.CreateDirectory(newPath);

                isSaved = true;
            }
            catch
            {
                isSaved = false;
            }

            if (isSaved)
            {
                Logger.SaveLog(new CreateDirectoryProvider(model));
            }
            else
                TempData["Error"] = ValidationResources.CreateDirectoryFailure;

            return RedirectToAction("List", new {currentUrl = _webHelper.MapUrl(newPath)});
        }

        [HttpGet]
        public ActionResult UploadFile(string targetUrl)
        {
            targetUrl = "/" + targetUrl;

            if (targetUrl.Contains("..") ||
                !targetUrl.StartsWith(Constants.CdnUrl))
                return NotFoundView();

            string targetPath = Server.MapPath(targetUrl);

            if ((!Directory.Exists(targetPath) && !System.IO.File.Exists(targetPath)))
                return NotFoundView();

            bool processParent = targetUrl != Constants.CdnUrl;

            return View(new UploadFileModel
                            {
                                TargetUrl = targetUrl,
                                CurrentEntry = new EntryModel(targetUrl, true, processParent)
                            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadFile(UploadFileModel model)
        {
            model.TargetUrl = "/" + model.TargetUrl;

            bool processParent = model.TargetUrl != Constants.CdnUrl;

            model.CurrentEntry = new EntryModel(model.TargetUrl, true, processParent);

            if (model.TargetUrl.Contains("..") ||
                !model.TargetUrl.StartsWith(Constants.CdnUrl))
                return NotFoundView();

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", ValidationResources.InvalidState);

                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.FileName))
                model.FileName = model.PostedFile.FileName;

            string targetPath = Server.MapPath(model.TargetUrl);

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

            bool isSaved;

            if (System.IO.File.Exists(Path.Combine(targetPath, model.FileName)))
            {
                ModelState.AddModelError("", ValidationResources.DuplicateFileName);

                return View(model);
            }

            try
            {
                UploadUtilities.Save(model.PostedFile, targetPath, model.FileName);

                isSaved = true;
            }
            catch
            {
                isSaved = false;
            }

            if (isSaved)
            {
                Logger.SaveLog(new UploadFileProvider(model));
                return RedirectToAction("List", new {currentUrl = model.TargetUrl});
            }

            ModelState.AddModelError("", ValidationResources.UploadFileFailure);
            return View(model);
        }

        [HttpGet]
        public ActionResult Delete(string targetUrl)
        {
            targetUrl = "/" + targetUrl;

            if (targetUrl.Contains("..") ||
                !targetUrl.StartsWith(Constants.CdnUrl))
                return NotFoundView();

            if (string.Compare(targetUrl, Constants.CdnUrl, true) == 0)
                return AccessDeniedView();

            string targetPath = Server.MapPath(targetUrl);

            if ((!Directory.Exists(targetPath) && !System.IO.File.Exists(targetPath)))
                return NotFoundView();

            bool isSaved = false;

            try
            {
                if (Directory.Exists(targetPath))
                    Directory.Delete(targetPath, true);
                else
                    System.IO.File.Delete(targetPath);

                isSaved = true;
            }
            catch
            {
            }

            if (isSaved)
            {
                Logger.SaveLog(new DeleteDirectoryOrFileProvider(targetPath));
            }
            else
                TempData["Error"] = ValidationResources.DeleteDirectoryOrFileFailure;

            DirectoryInfo parent = Directory.GetParent(targetPath);

            return RedirectToAction("List", new {currentUrl = _webHelper.MapUrl(parent.FullName)});
        }
    }
}