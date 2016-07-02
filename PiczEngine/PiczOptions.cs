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

        public int BackgroundAdjustmentPercent { get; set; }

        public static PiczOptions Load()
        {
            // Defaults
            var options = new PiczOptions
            {
                Route = "picz",
                Sizes = new List<int> { 4000, 2500, 1024, 640, 320 },
                BackgroundAdjustmentPercent = 10,
                CacheDurationHours = 48,
            };

            // Mandatory Config
            options.CacheRootPath = Config("PiczCachePath");

            // Optional Config
            var configRoute = Config("PiczRoute");
            if (!string.IsNullOrWhiteSpace(configRoute))
            {
                options.Route = configRoute;
            }

            var configSizes = Config("PiczSizes");
            if (!string.IsNullOrWhiteSpace(configSizes))
            {
                options.Sizes = configSizes.Split(',').Select(s => int.Parse(s)).ToList();
            }

            var configBackgroundPercent = Config("PiczBackgroundAdjustmentPercent");
            if (!string.IsNullOrWhiteSpace(configBackgroundPercent))
            {
                options.BackgroundAdjustmentPercent = int.Parse(configBackgroundPercent);
            }

            var configDuration = Config("PiczCacheDurationHours");
            if (!string.IsNullOrWhiteSpace(configDuration))
            {
                options.CacheDurationHours = int.Parse(configDuration);
            }

            return options;
        }

        private static string Config(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}