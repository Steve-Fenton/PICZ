namespace System.Web.Mvc.Html
{
    public static class BaseHelper
    {
        public static string GetImageHashForUrl(string hash)
        {
            if (string.IsNullOrWhiteSpace(hash))
            {
                return string.Empty;
            }

            return $"&h={hash}";
        }
    }
}
