using Fenton.Picz.Engine;
using System.Collections.Generic;
using System.Linq;

namespace System.Web.Mvc.Html
{
    public static class PiczHelper
    {
        public static MvcHtmlString Picz(this HtmlHelper helper, string url, string sizes, object htmlAttributes, string hash = "")
        {
            return Picz(helper, url, sizes, PiczOptions.Load(), htmlAttributes, hash);
        }

        public static MvcHtmlString PiczAppend(this HtmlHelper helper, string url, string sizes, object htmlAttributes, string hash = "")
        {
            return PiczAppend(helper, url, sizes, PiczOptions.Load(), htmlAttributes, hash);
        }

        public static MvcHtmlString Picz(this HtmlHelper helper, string url, string sizes, PiczOptions options, object htmlAttributes, string hash = "")
        {
            url = url.TrimStart(new char[] { '~' });

            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            var sourceSets = new List<string>();
            foreach (var size in options.Sizes)
            {
                sourceSets.Add($"/{options.Route}?s={size}&p={url}{BaseHelper.GetImageHashForUrl(hash)} {size}w");
            }

            var defaultSource = $"/{options.Route}?s={options.Sizes.Min()}&p={url}{BaseHelper.GetImageHashForUrl(hash)}";

            return BuildImageTag(sizes, attributes, sourceSets, defaultSource);
        }

        public static MvcHtmlString PiczAppend(this HtmlHelper helper, string url, string sizes, PiczOptions options, object htmlAttributes, string hash = "")
        {
            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            var sourceSets = new List<string>();
            foreach (var size in options.Sizes)
            {
                sourceSets.Add($"{url}?s={size}{BaseHelper.GetImageHashForUrl(hash)} {size}w");
            }

            var defaultSource = $"{url}?s={options.Sizes.Min()}{BaseHelper.GetImageHashForUrl(hash)}";

            return BuildImageTag(sizes, attributes, sourceSets, defaultSource);
        }

        private static MvcHtmlString BuildImageTag(string sizes, Web.Routing.RouteValueDictionary attributes, List<string> sourceSets, string defaultSource)
        {
            var builder = new TagBuilder("img");
            builder.MergeAttributes(attributes);
            builder.Attributes.Add("src", defaultSource);
            builder.Attributes.Add("srcset", string.Join(", ", sourceSets));
            builder.Attributes.Add("sizes", sizes);
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.SelfClosing));
        }
    }
}