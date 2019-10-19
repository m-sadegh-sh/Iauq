using System.Web;
using System.Web.Routing;

namespace Iauq.Information.Helpers
{
    public static class Constants
    {
        //public const string CdnUrl = "http://cdn.next.iauq.ac.ir/";

        public const string CdnUrl = "/cdn";

        public static string ImagesUrl = string.Format("{0}/images/", CdnUrl);
        public static string UploadsUrl = string.Format("{0}/uploads/", CdnUrl);
        public static string CarouselsUrl = string.Format("{0}carousels/", ImagesUrl);

        public static string StylesUrl = string.Format("{0}/styles/", CdnUrl);
        public static string ScriptsUrl = string.Format("{0}/scripts/", CdnUrl);

        public const int RecordPerPage = 15;
        public const int RecordPerPartial = 8;
    }
}