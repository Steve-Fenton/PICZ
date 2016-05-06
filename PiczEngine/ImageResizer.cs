using ImageProcessor;
using ImageProcessor.Imaging.Formats;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;

namespace Fenton.Picz.Engine
{
    public class ImageResizer
    {
        public ReplacementImage GetReplacementImage(int size, string originalUrl, Func<byte[]> getImage = null)
        {
            // Configuration values
            var options = PiczOptions.Load();

            size = ConstrainSize(size, options);

            // Create a cache folder that includes the image size
            var cacheFolderPath = Path.Combine(options.CacheRootPath, size.ToString());

            ReplacementImage replacementImage = GetSafeNamedImage(originalUrl, cacheFolderPath);

            FileInfo fileInfo = new FileInfo(replacementImage.Path);
            if (fileInfo.Exists && (fileInfo.LastWriteTimeUtc.AddHours(options.CacheDurationHours) > DateTime.UtcNow))
            {
                return replacementImage;
            }

            // Create image
            Directory.CreateDirectory(cacheFolderPath);

            if (getImage == null)
            {
                Resize(size, replacementImage.Path, originalUrl);
            }
            else
            {
                Resize(size, replacementImage.Path, getImage());
            }

            return replacementImage;
        }

        private static int ConstrainSize(int size, PiczOptions options)
        {
            // Important!
            // This prevents random sizes being requested, which would take up infinitely more
            // disk space than predicted - this method constrains the size to one of the configured
            // values.
            if (!options.Sizes.Contains(size))
            {
                size = options.Sizes.OrderBy(item => Math.Abs(size - item)).First();
            }

            return size;
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

        private void Resize(int width, string resizedPath, string originalPath)
        {
            byte[] photoBytes;
            using (var webClient = new WebClient())
            {
                photoBytes = webClient.DownloadData(originalPath);
            }

            Resize(width, resizedPath, photoBytes);
        }

        private void Resize(int width, string resizedPath, byte[] photoBytes)
        {
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
