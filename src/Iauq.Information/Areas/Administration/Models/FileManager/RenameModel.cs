namespace Iauq.Information.Areas.Administration.Models.FileManager
{
    public class RenameModel : RenameModelBase
    {
        public string TargetUrl { get; set; }

        public EntryModel CurrentEntry { get; set; }
    }
}