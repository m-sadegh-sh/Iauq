namespace Iauq.Information.Areas.Administration.Models.FileManager
{
    public class UploadFileModel : UploadFileModelBase
    {
        public string TargetUrl { get; set; }
        
        public EntryModel CurrentEntry { get; set; }
    }
}