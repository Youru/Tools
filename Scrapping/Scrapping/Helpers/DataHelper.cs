using Newtonsoft.Json;
using Scrapping.Domain.Model;
using System;
using System.Collections.Generic;
using System.IO;

namespace Scrapping.Helpers
{
    public static class DataHelper
    {

        public static IEnumerable<SiteSelector> GetSiteSelectors()
        {

            var pathFile = $"{AppDomain.CurrentDomain.BaseDirectory}\\DataSource\\sites.json";
            if (File.Exists(pathFile))
            {
                return JsonConvert.DeserializeObject<IEnumerable<SiteSelector>>(File.ReadAllText(pathFile));
            }
            else
            {
                throw new FileNotFoundException($"file not found at {pathFile}");
            }
        }
    }
}
