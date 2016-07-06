using Fenton.Picz.Engine;
using System.Linq;
using System.Text;

namespace System.Web.Mvc.Html
{
    public static class PiczBackgroundHelper
    {
        public static MvcHtmlString PiczBackground(this HtmlHelper helper, string url, string id, string hash = "")
        {
            return PiczBackground(helper, url, id, PiczOptions.Load(), hash);
        }

        public static MvcHtmlString PiczBackgroundAppend(this HtmlHelper helper, string url, string id, string hash = "")
        {
            return PiczBackgroundAppend(helper, url, id, PiczOptions.Load(), hash);
        }

        public static MvcHtmlString PiczBackground(this HtmlHelper helper, string url, string id, PiczOptions options, string hash = "")
        {
            url = url.TrimStart(new char[] { '~' });

            var builder = new StringBuilder();
            builder.AppendLine("<style scoped>");

            bool isDefaultSet = false;

            foreach (var size in options.Sizes.OrderByDescending(s => s))
            {
                var img = $"/{options.Route}?s={size}&p={url}{BaseHelper.GetImageHashForUrl(hash)}";
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

        public static MvcHtmlString PiczBackgroundAppend(this HtmlHelper helper, string url, string id, PiczOptions options, string hash = "")
        {
            var builder = new StringBuilder();
            builder.AppendLine("<style scoped>");

            bool isDefaultSet = false;

            foreach (var size in options.Sizes.OrderByDescending(s => s))
            {
                var img = $"{url}?s={size}{BaseHelper.GetImageHashForUrl(hash)}";
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