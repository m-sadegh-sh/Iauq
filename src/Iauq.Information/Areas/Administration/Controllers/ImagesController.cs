using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Iauq.Core.Domain;
using Iauq.Core.Utilities;
using Iauq.Data;
using Iauq.Data.Services;
using Iauq.Information.App_GlobalResources;
using Iauq.Information.Helpers;
using Iauq.Information.LogProviders;
using File = Iauq.Core.Domain.File;

namespace Iauq.Information.Areas.Administration.Controllers
{
    [CustomAuthorize(Roles = "Administrators, Moderators, Editors")]
    public class ImagesController : AdministrationControllerBase
    {
        private readonly IFileService _fileService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHelper _webHelper;

        public ImagesController(IFileService fileService, IUnitOfWork unitOfWork, IWebHelper webHelper)
        {
            _fileService = fileService;
            _unitOfWork = unitOfWork;
            _webHelper = webHelper;
        }

        //[ValidateAntiForgeryToken]
        public ActionResult Upload(HttpPostedFileBase upload, string CKEditorFuncNum, string CKEditor, string langCode)
        {
            string url;
            string message;
            string output;

            if (upload == null)
                return null;

            const string extensions =
                ".7z|.aiff|.asf|.avi|.bmp|.csv|.doc|.docx|.fla|.flv|.gif|.gz|.gzip|.jpeg|.jpg|.mid|.mov|.mp3|.mp4|.mpc|.mpeg|.mpg|.ods|.odt|.pdf|.png|.ppt|.pxd|.qt|.ram|.rar|.rm|.rmi|.rmvb|.rtf|.sdc|.sitd|.swf|.sxc|.sxw|.tar|.tgz|.tif|.tiff|.txt|.vsd|.wav|.wma|.wmv|.xls|.xml|.zip";

            if (upload.ContentLength == 0 || upload.ContentLength > 1000000 ||
                extensions.Split('|').All(e => e != Path.GetExtension(upload.FileName)) ||
                !UploadUtilities.IsValidImageBinary(upload.InputStream))
            {
                message = ValidationResources.SelectedFileIsInvalid;

                output = BuildOutput(CKEditorFuncNum, null, message);

                return Content(output);
            }

            var file = new File
                           {
                               Uploader = _webHelper.GetCurrentUser(HttpContext),
                               AccessMode = AccessMode.Any,
                               CreateDate = DateTime.UtcNow,
                               Name = upload.FileName,
                               ContentType = upload.ContentType,
                               Size = upload.ContentLength,
                               IsPublished = true
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

                UploadUtilities.Save(upload, targetPath, file.Guid.ToString());

                url = Url.RouteUrl("Download", new {file.Guid, fn = file.Name});

                message = ValidationResources.UploadFileSuccess;

                output = BuildOutput(CKEditorFuncNum, url, message);
                return Content(output);
            }

            message = ValidationResources.UploadFileFailure;

            output = BuildOutput(CKEditorFuncNum, null, message);
            return Content(output);
        }

        private string BuildOutput(string CKEditorFuncNum, string url, string message)
        {
            return @"<html><body><script>window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum + ", \"" +
                   url + "\", \"" + message + "\");</script></body></html>";
        }
    }
}