using Fenton.Picz.Engine;
using System.Linq;
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
                size: s.Value,
                originalUrl: Request.Url.AbsoluteUri.Split('?').FirstOrDefault() + ".jpg",
                hash: h,
                getImage: () => System.IO.File.ReadAllBytes(Server.MapPath(Url.Content("~/Content/paris.jpg"))),
                checkHash: () => "example-1");

            return File(replacementImage.Path, replacementImage.MimeType);
        }
    }
}