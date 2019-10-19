using System;
using System.IO;
using System.Text;
using System.Threading;

namespace Iauq.Web.Security
{
    public static class FileWriteHelper
    {
        public static void Write(string contents, string postfix)
        {
            const string directoryPath = "D:\\Iauq.Information\\Logs\\";

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            DateTime utcNow = DateTime.Now;

            string fileName = string.Format("{0}-{1}-{2} {3},{4} ({5}).log", utcNow.Year, utcNow.Month, utcNow.Day,
                                            utcNow.Hour, utcNow.Minute, postfix);

            string fullPath = Path.Combine(directoryPath, fileName);

            if (!File.Exists(fullPath))
                using (File.Create(fullPath))
                {
                }

            byte[] buffer = Encoding.UTF8.GetBytes(contents);

            int tries = 0;
            bool success = false;

            while (tries++ < 3 && !success)
                success = Write(fullPath, buffer);
        }

        private static bool Write(string fullPath, byte[] buffer)
        {
            try
            {
                using (FileStream file = File.Open(fullPath, FileMode.Append, FileAccess.Write))
                    file.Write(buffer, 0, buffer.Length);

                return true;
            }
            catch
            {
                Thread.Sleep(250);

                return false;
            }
        }
    }
}