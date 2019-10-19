using Iauq.Information.Models;

namespace Iauq.Information.Areas.Administration.Models.FileManager
{
    public class CreateDirectoryModel : CreateDirectoryModelBase
    {
        public string TargetUrl { get; set; }

        public EntryModel CurrentEntry { get; set; }
    }
}