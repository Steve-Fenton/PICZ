using Fenton.Picz.Engine;
using System.Linq;
using System.Text;

namespace System.Web.Mvc.Html
{
    public static class PiczBackgroundHelper
    {
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

        public static MvcHtmlString PiczBackgroundAppend(this HtmlHelper helper, string url, string id)
        {
            return PiczBackgroundAppend(helper, url, id, PiczOptions.Load());
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
                decimal breakpoint = GetBreakpoint(options, size);

                if (!isDefaultSet)
                {
                    builder.AppendLine($"#{id} {{ background-image: url(\"{img}\") }}");
                    isDefaultSet = true;
                }

                builder.AppendLine($"@media only screen and (max-width: {breakpoint}px) {{ #{id} {{ background-image: url(\"{img}\") }} }} ");
            }

            builder.AppendLine("</style>");
            return MvcHtmlString.Create(builder.ToString());
        }

        public static MvcHtmlString PiczBackgroundAppend(this HtmlHelper helper, string url, string id, PiczOptions options)
        {
            var builder = new StringBuilder();
            builder.AppendLine("<style scoped>");

            bool isDefaultSet = false;

            foreach (var size in options.Sizes.OrderByDescending(s => s))
            {
                var img = $"{url}?s={size}";
                var breakpoint = GetBreakpoint(options, size);

                if (!isDefaultSet)
                {
                    builder.AppendLine($"#{id} {{ background-image: url(\"{img}\") }}");
                    isDefaultSet = true;
                }

                builder.AppendLine($"@media only screen and (max-width: {breakpoint}px) {{ #{id} {{ background-image: url(\"{img}\") }} }} ");
            }

            builder.AppendLine("</style>");
            return MvcHtmlString.Create(builder.ToString());
        }

        private static decimal GetBreakpoint(PiczOptions options, int size)
        {
            return Math.Round((size / 100M) * (100M - options.BackgroundAdjustmentPercent), 0);
        }
    }
}