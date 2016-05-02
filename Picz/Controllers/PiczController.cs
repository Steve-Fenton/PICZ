using Fenton.Picz.Engine;
using System;
using System.Web.Mvc;

namespace Picz.Controllers
{
    public class PiczController : Controller
    {
        private readonly ImageResizer _imageResizer = new ImageResizer();

        [Route("Picz")]
        public ActionResult Picz(int s, string p)
        {
            // Optional - disk check is available here... you can use this to write to 
            // your preferred logging mechanism to get early warnings about disk space.
            var freeSpace = DiskInformation.GetDiskSpaceInSIMegabytes("C:\\");

            var originalUrl = new Uri(Request.Url, p).AbsoluteUri;
            var replacement = _imageResizer.GetReplacementImage(s, originalUrl);
            return File(replacement.Path, replacement.MimeType);
        }
    }
}