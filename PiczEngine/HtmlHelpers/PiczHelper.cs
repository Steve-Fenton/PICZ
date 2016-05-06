using Fenton.Picz.Engine;
using System.Collections.Generic;
using System.Linq;

namespace System.Web.Mvc.Html
{
    public static class PiczHelper
    {
        /// <summary>
        /// Generates an image with responsive source sets
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="url">The URL of the image</param>
        /// <param name="sizes">The size of the image in the display, for example 100vw for full width</param>
        /// <param name="htmlAttributes">Additional attributes for the image (recommended: alt)</param>
        /// <returns></returns>
        public static MvcHtmlString Picz(this HtmlHelper helper, string url, string sizes, object htmlAttributes)
        {
            return Picz(helper, url, sizes, PiczOptions.Load(), htmlAttributes);
        }

        public static MvcHtmlString PiczAppend(this HtmlHelper helper, string url, string sizes, object htmlAttributes)
        {
            return PiczAppend(helper, url, sizes, PiczOptions.Load(), htmlAttributes);
        }

        public static MvcHtmlString Picz(this HtmlHelper helper, string url, string sizes, PiczOptions options, object htmlAttributes)
        {
            url = url.TrimStart(new char[] { '~' });

            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            var sourceSets = new List<string>();
            foreach (var size in options.Sizes)
            {
                sourceSets.Add($"/{options.Route}?s={size}&p={url} {size}w");
            }

            var defaultSource = $"/{options.Route}?s={options.Sizes.Min()}&p={url}";

            return BuildImageTag(sizes, attributes, sourceSets, defaultSource);
        }

        public static MvcHtmlString PiczAppend(this HtmlHelper helper, string url, string sizes, PiczOptions options, object htmlAttributes)
        {
            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            var sourceSets = new List<string>();
            foreach (var size in options.Sizes)
            {
                sourceSets.Add($"{url}?s={size} {size}w");
            }

            var defaultSource = $"{url}?s={options.Sizes.Min()}";

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