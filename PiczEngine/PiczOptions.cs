using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Fenton.Picz.Engine
{
    public class PiczOptions
    {
        public string Route { get; set; }

        public IList<int> Sizes { get; set; }

        public int CacheDurationHours { get; set; }

        public string CacheRootPath { get; set; }

        public static PiczOptions Load()
        {
            var options = new PiczOptions
            {
                Route = "picz",
                Sizes = new List<int> { 4000, 2500, 1024, 640, 320 }
            };

            var configRoute = ConfigurationManager.AppSettings["PiczRoute"];
            if (!string.IsNullOrWhiteSpace(configRoute))
            {
                options.Route = configRoute;
            }

            var configSizes = ConfigurationManager.AppSettings["PiczSizes"];
            if (!string.IsNullOrWhiteSpace(configSizes))
            {
                options.Sizes = configSizes.Split(',').Select(s => int.Parse(s)).ToList();
            }

            options.CacheDurationHours = int.Parse(ConfigurationManager.AppSettings["PiczCacheDurationHours"]);

            options.CacheRootPath = ConfigurationManager.AppSettings["PiczCachePath"];

            return options;
        }
    }
}
