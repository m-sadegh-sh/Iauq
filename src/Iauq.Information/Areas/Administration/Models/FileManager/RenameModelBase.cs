using System.ComponentModel;
using Iauq.Information.Areas.Administration.Helpers;
using Iauq.Information.Models;

namespace Iauq.Information.Areas.Administration.Models.FileManager
{
    public class RenameModelBase : ModelBase
    {
        [DisplayName("نام فعلی")]
        public string OldName { get; set; }

        [DisplayName("نام جدید")]
        [FileNameValidator]
        public string NewName { get; set; }
    }
}