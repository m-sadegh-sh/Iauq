using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Iauq.Information.Areas.Administration.Helpers;
using Iauq.Information.Models;

namespace Iauq.Information.Areas.Administration.Models.FileManager
{
    public abstract class UploadFileModelBase : ModelBase
    {
        [DisplayName("نام")]
        [FileNameValidator]
        public string FileName { get; set; }

        [DisplayName("فایل")]
        [Required(ErrorMessage = "لطفا فایل مورد نظر را انتخاب کنید.")]
        public HttpPostedFileBase PostedFile { get; set; }

        public string ContentType
        {
            get
            {
                if (PostedFile == null)
                    return null;

                return PostedFile.ContentType;
            }
        }

        public int ContentLength
        {
            get
            {
                if (PostedFile == null)
                    return -1;

                return PostedFile.ContentLength;
            }
        }
    }
}