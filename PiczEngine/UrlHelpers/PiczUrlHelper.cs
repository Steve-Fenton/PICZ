using Fenton.Picz.Engine;
using System.Linq;
using System.Web.Mvc.Html;

namespace System.Web.Mvc
{
    public static class PiczUrlHelper
    {
        public static string PiczUrl(this UrlHelper helper, string url, string hash = "")
        {
            var options = PiczOptions.Load();
            var size = options.Sizes.OrderByDescending(s => s).FirstOrDefault();

            return PiczUrl(helper, url, size, hash);
        }

        public static string PiczUrl(this UrlHelper helper, string url, int size, string hash = "")
        {
            url = url.TrimStart(new char[] { '~' });

            return $"{url}?s={size}{BaseHelper.GetImageHashForUrl(hash)}";
        }
    }
}
