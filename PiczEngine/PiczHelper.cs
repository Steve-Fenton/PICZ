using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace System.Web.Mvc.Html
{
    public static class PiczHelper
    {
        public static PiczOptions DefaultOptions = new PiczOptions
        {
            Sizes = new List<int> { 2500, 1024, 640, 320 }
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
            builder.Attributes.Add("src", $"/picz?s={options.Sizes.Min()}&p={url}");
            builder.Attributes.Add("srcset", string.Join(", ", sourceSets));
            builder.Attributes.Add("sizes", sizes);
            return MvcHtmlString.Create(builder.ToString());
        }

        private static IList<string> GetSourceSets(string url, PiczOptions options)
        {
            var sourceSets = new List<string>();

            foreach (var size in options.Sizes)
            {
                sourceSets.Add($"/picz?s={size}&p={url} {size}w");
            }

            return sourceSets;
        }

        private static PiczOptions GetPiczOptions()
        {
            var configSizes = ConfigurationManager.AppSettings["PiczSized"];
            if (!string.IsNullOrWhiteSpace(configSizes))
            {
                var configOptions = new PiczOptions
                {
                    Sizes = configSizes.Split(',').Select(s => int.Parse(s)).ToList()
                };

                return configOptions;
            }

            return DefaultOptions;

        }

    }

    public class PiczOptions
    {
        public IList<int> Sizes { get; set; }
    }
}
