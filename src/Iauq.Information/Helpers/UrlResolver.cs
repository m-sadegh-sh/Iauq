using System.IO;
using System.Linq;
using System.Web;

namespace Iauq.Information.Helpers
{
    public static class UrlResolver
    {
        public static string Cdn(string filename)
        {
            return string.Format("{0}{1}", Constants.CdnUrl, filename);
        }

        public static string Style(string filename, bool minified = true)
        {
            return string.Format("{0}{1}{2}{3}", Constants.StylesUrl, filename, minified ? ".min" : null, ".css");
        }

        public static string Script(string filename, bool minified = true)
        {
            return string.Format("{0}{1}{2}{3}", Constants.ScriptsUrl, filename, minified ? ".min" : null, ".js");
        }

        public static string Carousel(string filename)
        {
            string directory = HttpContext.Current.Server.MapPath(Constants.CarouselsUrl);

            if (!Directory.Exists(directory))
                return "";

            string fullPath = Directory.GetFiles(directory, filename + "*").ToList().FirstOrDefault();

            if (fullPath == null || !File.Exists(fullPath))
                return "";

            string fileNameWithExtension = Path.GetFileName(fullPath);

            fileNameWithExtension = fileNameWithExtension.Replace("-thumb", "");

            return string.Format("{0}{1}", Constants.CarouselsUrl, fileNameWithExtension);
        }
    }
}