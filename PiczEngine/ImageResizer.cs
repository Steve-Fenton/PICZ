using ImageProcessor;
using ImageProcessor.Imaging.Formats;
using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Net;
using System.Web;

namespace Fenton.Picz.Engine
{
    public class ImageResizer
    {
        public ReplacementImage GetReplacementImage(int size, string originalUrl)
        {
            // Configuration values
            var cacheDurationHours = int.Parse(ConfigurationManager.AppSettings["PiczCacheDurationHours"]);
            var cacheRootPath = ConfigurationManager.AppSettings["PiczCachePath"];

            // Create a cache folder that includes the image size
            var cacheFolderPath = Path.Combine(cacheRootPath, size.ToString());

            ReplacementImage replacementImage = GetSafeNamedImage(originalUrl, cacheFolderPath);

            FileInfo fileInfo = new FileInfo(replacementImage.Path);
            if (fileInfo.Exists
                && (fileInfo.LastWriteTimeUtc.AddHours(cacheDurationHours) > DateTime.UtcNow))
            {
                return replacementImage;
            }

            // Create image
            Directory.CreateDirectory(cacheFolderPath);
            Resize(originalUrl, replacementImage.Path, size);

            return replacementImage;
        }

        private static ReplacementImage GetSafeNamedImage(string originalUrl, string cacheFolderPath)
        {
            var invalids = Path.GetInvalidFileNameChars();
            var name = string.Join("__", originalUrl.Split(invalids, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');

            var fullPath = Path.Combine(cacheFolderPath, name);

            var replacementImage = new ReplacementImage
            {
                Path = fullPath,
                MimeType = "image/jpeg"
            };

            return replacementImage;
        }

        private void Resize(string originalPath, string resizedPath, int width)
        {
            byte[] photoBytes;
            using (var webClient = new WebClient())
            {
                photoBytes = webClient.DownloadData(originalPath);
            }

            // Format is automatically detected though can be changed.
            ISupportedImageFormat format = new JpegFormat { Quality = 90 };
            Size size = new Size(width, 0);

            using (var inStream = new MemoryStream(photoBytes))
            using (var fileStream = new FileStream(resizedPath, FileMode.Create, FileAccess.Write))
            using (var imageFactory = new ImageFactory())
            {
                // Load, resize, set the format and quality and save an image.
                imageFactory
                    .Load(inStream)
                    .Resize(size)
                    .Format(format)
                    .Save(fileStream);
            }
        }
    }
}
