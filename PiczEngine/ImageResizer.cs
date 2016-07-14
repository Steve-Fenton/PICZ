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
        public ReplacementImage GetReplacementImage(int size, string originalUrl, string hash, Func<byte[]> getImage = null, Func<string> checkHash = null)
        {
            // Configuration values
            var options = PiczOptions.Load();
            bool hasHash = !string.IsNullOrWhiteSpace(hash);

            size = ConstrainSize(size, options);

            // Create a cache folder that includes the image size
            var cacheFolderPath = Path.Combine(options.CacheRootPath, size.ToString());

            ReplacementImage replacement = null;

            if (hasHash)
            {
                cacheFolderPath = Path.Combine(cacheFolderPath, "hashvalidated");

                replacement = GetSafeNamedImage(hash, originalUrl, cacheFolderPath);

                var fileInfo = new FileInfo(replacement.Path);

                // Cache Miss - chance the hash has been fiddled so double check it if a validator is supplied
                if (!fileInfo.Exists && checkHash != null)
                {
                    var checkedHash = checkHash();

                    if (checkedHash != hash)
                    {
                        // Ah - the hash is not right, so let's try again with the correct one
                        // this stops us from creating lots of copies of the same image every time
                        // a robot sends a different hash
                        hash = checkedHash;
                        replacement = GetSafeNamedImage(hash, originalUrl, cacheFolderPath);
                        fileInfo = new FileInfo(replacement.Path);
                    }
                }

                if (fileInfo.Exists)
                {
                    return replacement;
                }
            }
            else
            {
                replacement = GetSafeNamedImage(hash, originalUrl, cacheFolderPath);

                var fileInfo = new FileInfo(replacement.Path);

                if (fileInfo.Exists)
                {
                    if (fileInfo.LastWriteTimeUtc.AddHours(options.CacheDurationHours) > DateTime.UtcNow)
                    {
                        // If no hash is supplied, check the file is new enough to use
                        return replacement;
                    }
                }
            }

            // Create image
            Directory.CreateDirectory(cacheFolderPath);

            if (getImage == null)
            {
                Resize(size, replacement, originalUrl, options);
            }
            else
            {
                Resize(size, replacement, getImage(), options);
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

        private static ReplacementImage GetSafeNamedImage(string hash, string originalUrl, string cacheFolderPath)
        {
            string name = RemoveInvalidCharacters(hash) + "_" + RemoveInvalidCharacters(originalUrl);

            var fullPath = Path.Combine(cacheFolderPath, name);

            var replacementImage = new ReplacementImage
            {
                Path = fullPath,
                MimeType = MimeMapping.GetMimeMapping(originalUrl)
            };

            return replacementImage;
        }

        private static string RemoveInvalidCharacters(string unsafeString)
        {
            var invalids = Path.GetInvalidFileNameChars();
            var safeString = string.Join("__", unsafeString.Split(invalids, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
            return safeString;
        }

        private void Resize(int width, ReplacementImage replacement, string originalPath, PiczOptions options)
        {
            byte[] photoBytes;
            using (var webClient = new WebClient())
            {
                photoBytes = webClient.DownloadData(originalPath);
            }

            Resize(width, replacement, photoBytes, options);
        }

        private void Resize(int width, ReplacementImage replacement, byte[] photoBytes, PiczOptions options)
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
                    .Format(GetFormat(replacement, options))
                    .Quality(options.Quality)
                    .Save(fileStream);
            }
        }

        private static ISupportedImageFormat GetFormat(ReplacementImage replacement, PiczOptions options)
        {
            switch (replacement.MimeType)
            {
                case "image/png":
                    return new PngFormat { Quality = options.Quality };

                case "image/gif":
                    return new GifFormat { Quality = options.Quality };

                default:
                    return new JpegFormat { Quality = options.Quality };
            }
        }
    }
}