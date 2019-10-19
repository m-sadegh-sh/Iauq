using System;
using System.ComponentModel;
using System.IO;
using System.Web;
using Iauq.Core.Utilities;
using Iauq.Information.Helpers;
using Iauq.Information.Models;
using StructureMap;

namespace Iauq.Information.Areas.Administration.Models.FileManager
{
    public class EntryModel : ModelBase
    {
        public EntryModel(string urlOrPath, bool isUrl, bool processParent)
        {
            if(string.IsNullOrEmpty(urlOrPath))
                return;

            var webHelper = ObjectFactory.GetInstance<IWebHelper>();

            if (isUrl)
            {
                CurrentUrl = urlOrPath;
                CurrentPath = HttpContext.Current.Server.MapPath(urlOrPath);
            }
            else
            {
                CurrentUrl = webHelper.MapUrl(urlOrPath);
                CurrentPath = urlOrPath;
            }

            IsDirectory = Directory.Exists(CurrentPath);
            IsFile = File.Exists(CurrentPath);

            if (processParent)
            {
                DirectoryInfo parent = Directory.GetParent(CurrentPath);

                if (parent != null)
                {
                    string parentUrl = webHelper.MapUrl(parent.FullName);
                    if (parentUrl.StartsWith(Constants.CdnUrl))
                    {
                        ParentPath = parent.FullName;
                        ParentUrl = parentUrl;
                    }
                }
            }

            if (IsDirectory)
            {
                CreationTime = Directory.GetCreationTime(CurrentPath);
            }
            else
            {
                CreationTime = File.GetCreationTime(CurrentPath);
            }
        }

        public bool IsDirectory { get; set; }
        public bool IsFile { get; set; }

        public string CurrentUrl { get; set; }
        public string CurrentPath { get; set; }
        public string CurrentName { get; set; }

        public string ParentUrl { get; set; }
        public string ParentPath { get; set; }
        public string ParentName { get; set; }

        [DisplayName("تاریخ ایجاد")]
        public DateTime CreationTime { get; set; }
    }
}