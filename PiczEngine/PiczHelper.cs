using Fenton.Picz.Engine;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

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

        /// <summary>
        /// Generates an image with responsive source sets
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="url">The URL of the image</param>
        /// <param name="id">The id of the image to target</param>
        /// <returns></returns>
        public static MvcHtmlString PiczBackground(this HtmlHelper helper, string url, string id)
        {
            return PiczBackground(helper, url, id, PiczOptions.Load());
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

            var builder = new TagBuilder("img");
            builder.MergeAttributes(attributes);
            builder.Attributes.Add("src", $"/{options.Route}?s={options.Sizes.Min()}&p={url}");
            builder.Attributes.Add("srcset", string.Join(", ", sourceSets));
            builder.Attributes.Add("sizes", sizes);
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.SelfClosing));
        }

        public static MvcHtmlString PiczBackground(this HtmlHelper helper, string url, string id, PiczOptions options)
        {
            url = url.TrimStart(new char[] { '~' });

            var builder = new StringBuilder();
            builder.AppendLine("<style scoped>");

            bool isDefaultSet = false;

            foreach (var size in options.Sizes.OrderByDescending(s => s))
            {
                var img = $"/{options.Route}?s={size}&p={url}";

                if (!isDefaultSet)
                {
                    builder.AppendLine($"#{id} {{ background-image: url(\"{img}\") }}");
                    isDefaultSet = true;
                }

                builder.AppendLine($"@media only screen and (max-width: {size}px) {{ #{id} {{ background-image: url(\"{img}\") }} }} ");
            }

            builder.AppendLine("</style>");
            return MvcHtmlString.Create(builder.ToString());
        }
    }
}