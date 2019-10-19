using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Iauq.Information.Areas.Administration.Models.FileManager;

namespace Iauq.Information.Areas.Administration.Models.Files
{
    public class UploadDbFileModel : UploadFileModelBase
    {
        public int? ParentId { get; set; }

        [DisplayName("نحوه دسترسی")]
        [Required(ErrorMessage = "ذکر نحوه دسترسی ضروری است.")]
        public short AccessMode { get; set; }
    }
}