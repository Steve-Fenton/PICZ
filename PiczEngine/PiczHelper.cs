using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace System.Web.Mvc.Html
{
    public static class PiczHelper
    {
        public static PiczOptions DefaultOptions = new PiczOptions
        {
            Route = "picz",
            Sizes = new List<int> { 4000, 2500, 1024, 640, 320 }
        };

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
            return Picz(helper, url, sizes, GetPiczOptions(), htmlAttributes);
        }

        public static MvcHtmlString Picz(this HtmlHelper helper, string url, string sizes, PiczOptions options, object htmlAttributes)
        {
            url = url.TrimStart(new char[] { '~' });

            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            IList<string> sourceSets = GetSourceSets(url, options);

            var builder = new TagBuilder("img");
            builder.MergeAttributes(attributes);
            builder.Attributes.Add("src", $"/{options.Route}?s={options.Sizes.Min()}&p={url}");
            builder.Attributes.Add("srcset", string.Join(", ", sourceSets));
            builder.Attributes.Add("sizes", sizes);
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.SelfClosing));
        }

        private static IList<string> GetSourceSets(string url, PiczOptions options)
        {
            var sourceSets = new List<string>();

            foreach (var size in options.Sizes)
            {
                sourceSets.Add($"/{options.Route}?s={size}&p={url} {size}w");
            }

            return sourceSets;
        }

        private static PiczOptions GetPiczOptions()
        {
            var options = new PiczOptions
            {
                Route = DefaultOptions.Route,
                Sizes = DefaultOptions.Sizes
            };

            var configRoute = ConfigurationManager.AppSettings["PiczRoute"];
            if (!string.IsNullOrWhiteSpace(configRoute))
            {
                options.Route = configRoute;
            }

            var configSizes = ConfigurationManager.AppSettings["PiczSized"];
            if (!string.IsNullOrWhiteSpace(configSizes))
            {
                options.Sizes = configSizes.Split(',').Select(s => int.Parse(s)).ToList();
            }

            return options;

        }

    }

    public class PiczOptions
    {
        public string Route { get; set; }

        public IList<int> Sizes { get; set; }
    }
}
