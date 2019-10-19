using System;
using System.Drawing;
using System.IO;
using System.Web;

namespace Iauq.Core.Utilities
{
    public static class UploadUtilities
    {
        public static bool IsValidImageBinary(Stream stream)
        {
            try
            {
                Image.FromStream(stream);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void TryToSaveImage(HttpPostedFileBase file, string directory, string fileName)
        {
            try
            {
                Image image = Image.FromStream(file.InputStream);

                string extension = Path.GetExtension(file.FileName);

                string originalImagePath = PrepareDirectoryAndBuildFileName(directory, fileName, extension, false);

                image.Save(originalImagePath);

                string thumbImagePath = PrepareDirectoryAndBuildFileName(directory, fileName, extension, true);

                ResizeImage(originalImagePath, thumbImagePath, 100, 150, true);
            }
            catch
            {
            }
        }

        public static string TryToSaveImage(HttpPostedFileBase file, string directory)
        {
            try
            {
                Image image = Image.FromStream(file.InputStream);

                string relativeFilePath;
                string randomFileName = Path.GetRandomFileName();

                string originalImagePath = PrepareDirectoryAndBuildRandomFileName(directory, randomFileName,
                                                                                  file.FileName, false,
                                                                                  out relativeFilePath);

                image.Save(originalImagePath);

                string t;
                string thumbImagePath = PrepareDirectoryAndBuildRandomFileName(directory, randomFileName, file.FileName,
                                                                               true,
                                                                               out t);

                ResizeImage(originalImagePath, thumbImagePath, 100, 150, true);

                return relativeFilePath;
            }
            catch
            {
                return null;
            }
        }

        private static string PrepareDirectoryAndBuildFileName(string directory, string fileName, string extension,
                                                               bool isThumb)
        {
            directory = HttpContext.Current.Server.MapPath(directory);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            directory = directory + fileName + (isThumb ? "-thumb" : "") + extension;

            if (File.Exists(directory))
                File.Delete(directory);

            return directory;
        }

        private static string PrepareDirectoryAndBuildRandomFileName(string directory, string randomFileName,
                                                                     string originalFileName,
                                                                     bool isThumb,
                                                                     out string relativeFilePath)
        {
            directory = HttpContext.Current.Server.MapPath(directory);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            string extension = Path.GetExtension(originalFileName);

            directory = directory + randomFileName + (isThumb ? "-thumb" : "") + extension;

            relativeFilePath = directory;

            //directory = HttpContext.Current.Server.MapPath(directory);

            if (File.Exists(directory))
                File.Delete(directory);

            return directory;
        }

        public static void ResizeImage(string originalFile, string newFile, int newWidth, int maxHeight,
                                       bool onlyResizeIfWider)
        {
            Image originalImage = Image.FromFile(originalFile);

            if (onlyResizeIfWider)
            {
                if (originalImage.Width <= newWidth)
                {
                    newWidth = originalImage.Width;
                }
            }

            int newHeight = originalImage.Height*newWidth/originalImage.Width;
            if (newHeight > maxHeight)
            {
                newWidth = originalImage.Width*maxHeight/originalImage.Height;
                newHeight = maxHeight;
            }

            Image newImage = originalImage.GetThumbnailImage(newWidth, newHeight, null, IntPtr.Zero);

            originalImage.Dispose();

            newImage.Save(newFile);
        }

        public static void Save(HttpPostedFileBase file, string path, string fileName)
        {
            string fullPathPath = Path.Combine(path, fileName);

            file.SaveAs(fullPathPath);
        }
    }
}