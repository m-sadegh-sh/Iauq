using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Web;
using Iauq.Core.Utilities;
using Iauq.Information.Models;
using StructureMap;

namespace Iauq.Information.Areas.Administration.Models.FileManager
{
    public class FileManagerModel : ModelBase
    {
        public FileManagerModel()
        {
            Entries = new List<EntryModel>();
        }

        public EntryModel CurrentEntry { get; set; }

        [DisplayName("مسیر/فایل‌های جاری")]
        public List<EntryModel> Entries { get; set; }

        public static string ExtractParent(string url)
        {
            string path = HttpContext.Current.Server.MapPath(url);

            var webHelper = ObjectFactory.GetInstance<IWebHelper>();

            DirectoryInfo parent = Directory.GetParent(path);
            return webHelper.MapUrl(parent.FullName);
        }

        public static string EnsureIsDirectoryOrExtractParent(string url)
        {
            string path = HttpContext.Current.Server.MapPath(url);

            var webHelper = ObjectFactory.GetInstance<IWebHelper>();

            if (Directory.Exists(path))
                return webHelper.MapUrl(path);

            DirectoryInfo parent = Directory.GetParent(path);
            return webHelper.MapUrl(parent.FullName);
        }

        public static string ExtractName(string url)
        {
            string path;
            if (Directory.Exists(url))
                path = url;
            else
                path = HttpContext.Current.Server.MapPath(url);

            if (Directory.Exists(path))
                return new DirectoryInfo(path).Name;

            return new FileInfo(path).Name;
        }
    }
}