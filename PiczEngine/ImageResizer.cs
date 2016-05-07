using ImageProcessor;
using ImageProcessor.Imaging.Formats;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

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

            ReplacementImage replacement = GetSafeNamedImage(originalUrl, cacheFolderPath);

            FileInfo fileInfo = new FileInfo(replacement.Path);
            if (fileInfo.Exists && (fileInfo.LastWriteTimeUtc.AddHours(options.CacheDurationHours) > DateTime.UtcNow))
            {
                return replacement;
            }

            // Create image
            Directory.CreateDirectory(cacheFolderPath);

            if (getImage == null)
            {
                Resize(size, replacement, originalUrl);
            }
            else
            {
                Resize(size, replacement, getImage());
            }

            return replacement;
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
                MimeType = MimeMapping.GetMimeMapping(originalUrl)
            };

            return replacementImage;
        }

        private void Resize(int width, ReplacementImage replacement, string originalPath)
        {
            byte[] photoBytes;
            using (var webClient = new WebClient())
            {
                photoBytes = webClient.DownloadData(originalPath);
            }

            Resize(width, replacement, photoBytes);
        }

        private void Resize(int width, ReplacementImage replacement, byte[] photoBytes)
        {
            using (var inStream = new MemoryStream(photoBytes))
            using (var fileStream = new FileStream(replacement.Path, FileMode.Create, FileAccess.Write))
            using (var imageFactory = new ImageFactory())
            {
                // No upscaling! No point creating images larger than the original
                using (var image = Image.FromStream(inStream))
                {
                    if (image.Width < width)
                    {
                        width = image.Width;
                    }
                }

                inStream.Position = 0;

                var size = new Size(width, 0);

                // Load, resize, set the format and quality and save an image.
                imageFactory
                    .Load(inStream)
                    .Resize(size)
                    .Format(GetFormat(replacement))
                    .Save(fileStream);
            }
        }

        private static ISupportedImageFormat GetFormat(ReplacementImage replacement)
        {
            switch (replacement.MimeType)
            {
                case "image/png":
                    return new PngFormat { Quality = 90 };
                case "image/gif":
                    return new GifFormat { Quality = 90 };
                default:
                    return new JpegFormat { Quality = 90 };
            }
        }
    }
}
