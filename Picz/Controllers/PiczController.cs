using Fenton.Picz.Engine;
using System;
using System.Web.Mvc;

namespace Picz.Controllers
{
    public class PiczController : Controller
    {
        private readonly ImageResizer _imageResizer = new ImageResizer();

        [Route("picz")]
        public ActionResult Picz(int s, string p, string h = "")
        {
            var originalUrl = new Uri(Request.Url, p).AbsoluteUri;
            var replacement = _imageResizer.GetReplacementImage(s, originalUrl, h);
            return File(replacement.Path, replacement.MimeType);
        }
    }
}