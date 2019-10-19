using System;
using System.IO;
using System.Web;

namespace Iauq.Web.Security
{
    public static class HttpRequestExtensions
    {
        public static void ToRaw(this HttpRequest request)
        {
            using (var writer = new StringWriter())
            {
                writer.Write(Environment.NewLine + Environment.NewLine + "///// ------------------------ ////");
                writer.Write("///// Start at {0} ////", DateTime.Now);

                WriteStartLine(request, writer);
                //WriteServerVariables(request, writer);
                //WriteHeaders(request, writer);
                WriteCookies(request, writer);
                WriteQueryString(request, writer);
                WriteForm(request, writer);
                WriteFiles(request, writer);
                WriteBody(request, writer);

                writer.Write(Environment.NewLine + Environment.NewLine + "///// END ////");

                FileWriteHelper.Write(writer.ToString(), "REQ");
            }
        }

        private static void WriteStartLine(HttpRequest request, TextWriter writer)
        {
            writer.Write(Environment.NewLine + "HttpMethod: " + request.HttpMethod);
            writer.Write(Environment.NewLine + "Url: " + request.Url);
            writer.WriteLine(Environment.NewLine + "RawUrl: " + request.RawUrl);
            writer.WriteLine(Environment.NewLine + "ContentType: " + request.ContentType);
            writer.WriteLine(Environment.NewLine + "FilePath: " + request.FilePath);
            writer.WriteLine(Environment.NewLine + "IsAuthenticated: " + request.IsAuthenticated);
            writer.WriteLine(Environment.NewLine + "IsLocal: " + request.IsLocal);
            writer.WriteLine(Environment.NewLine + "RequestType: " + request.RequestType);
            writer.WriteLine(Environment.NewLine + "TotalBytes: " + request.TotalBytes);
            writer.WriteLine(Environment.NewLine + "UserAgent: " + request.UserAgent);
            writer.WriteLine(Environment.NewLine + "UserHostName: " + request.UserHostName);
        }

        private static void WriteServerVariables(HttpRequest request, TextWriter writer)
        {
            writer.Write(Environment.NewLine + Environment.NewLine + "ServerVariables:");

            foreach (string key in request.ServerVariables.AllKeys)
                writer.WriteLine(string.Format("{0}{1}: {2}", Environment.NewLine, key, request.ServerVariables[key]));

            writer.WriteLine();
        }

        private static void WriteHeaders(HttpRequest request, TextWriter writer)
        {
            writer.Write(Environment.NewLine + Environment.NewLine + "Headers:");

            foreach (string key in request.Headers.AllKeys)
                writer.WriteLine(string.Format("{0}{1}: {2}", Environment.NewLine, key, request.Headers[key]));

            writer.WriteLine();
        }

        private static void WriteCookies(HttpRequest request, TextWriter writer)
        {
            writer.Write(Environment.NewLine + Environment.NewLine + "Cookies:");

            foreach (string key in request.Cookies.AllKeys)
                writer.WriteLine(string.Format("{0}{1}: {2}", Environment.NewLine, key, request.Cookies[key].Value));

            writer.WriteLine();
        }

        private static void WriteQueryString(HttpRequest request, TextWriter writer)
        {
            writer.Write(Environment.NewLine + Environment.NewLine + "QueryString:");

            foreach (string key in request.QueryString.AllKeys)
                writer.WriteLine(string.Format("{0}{1}: {2}", Environment.NewLine, key, request.QueryString[key]));

            writer.WriteLine();
        }

        private static void WriteForm(HttpRequest request, TextWriter writer)
        {
            writer.Write(Environment.NewLine + Environment.NewLine + "Form:");

            foreach (string key in request.Form.AllKeys)
                writer.WriteLine(string.Format("{0}{1}: {2}", Environment.NewLine, key, request.Form[key]));

            writer.WriteLine();
        }

        private static void WriteFiles(HttpRequest request, TextWriter writer)
        {
            writer.Write(Environment.NewLine + Environment.NewLine + "Files:");

            foreach (string key in request.Files.AllKeys)
                writer.WriteLine(string.Format("{0}{1}: {2}", Environment.NewLine, key, request.Files[key]));

            writer.WriteLine();
        }

        private static void WriteBody(HttpRequest request, TextWriter writer)
        {
            var reader = new StreamReader(request.InputStream);

            try
            {
                string body = reader.ReadToEnd();
                writer.WriteLine(Environment.NewLine + body);
            }
            finally
            {
                reader.BaseStream.Position = 0;
            }
        }
    }
}