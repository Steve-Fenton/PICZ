using Fenton.Picz.Engine;
using System.Web.Mvc;

namespace Picz.Controllers
{
    public class ImageController : Controller
    {
        private readonly ImageResizer _imageResizer = new ImageResizer();
        private int _defaultSize = 640;

        public ActionResult Index(int? s, string h = "")
        {
            if (!s.HasValue)
            {
                s = _defaultSize;
            }

            var replacementImage = _imageResizer.GetReplacementImage(
                s.Value,
                Request.Url.AbsoluteUri + ".jpg",
                h,
                () => System.IO.File.ReadAllBytes(Server.MapPath(Url.Content("~/Content/paris.jpg"))));

            return File(replacementImage.Path, replacementImage.MimeType);
        }
    }
}