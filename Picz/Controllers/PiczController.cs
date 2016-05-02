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
            var originalUrl = new Uri(Request.Url, p).AbsoluteUri;
            var replacement = _imageResizer.GetReplacementImage(s, originalUrl);
            return File(replacement.Path, replacement.MimeType);
        }
    }
}