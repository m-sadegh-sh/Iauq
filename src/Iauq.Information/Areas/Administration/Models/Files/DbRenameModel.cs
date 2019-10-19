using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Iauq.Information.Areas.Administration.Models.FileManager;

namespace Iauq.Information.Areas.Administration.Models.Files
{
    public class DbRenameModel : RenameModelBase
    {
        public int? ParentId { get; set; }
        public int Id { get; set; }

        [DisplayName("نحوه دسترسی")]
        [Required(ErrorMessage = "ذکر نحوه دسترسی ضروری است.")]
        public short AccessMode { get; set; }
    }
}