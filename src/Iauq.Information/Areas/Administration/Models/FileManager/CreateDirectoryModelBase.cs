using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Iauq.Information.Areas.Administration.Helpers;
using Iauq.Information.Models;

namespace Iauq.Information.Areas.Administration.Models.FileManager
{
    public abstract class CreateDirectoryModelBase : ModelBase
    {
        [DisplayName("نام")]
        [Required(ErrorMessage = "ذکر نام ضروری است.")]
        [FileNameValidator]
        public string Name { get; set; }
    }
}