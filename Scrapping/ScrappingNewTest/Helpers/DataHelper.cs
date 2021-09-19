using Newtonsoft.Json;
using ScrappingNewTest.Model;
using System;
using System.Collections.Generic;
using System.IO;

namespace ScrappingNewTest.Helpers
{
    public static class DataHelper
    {

        public static IEnumerable<Site> GetSites()
        {

            var pathFile = $"{AppDomain.CurrentDomain.BaseDirectory}\\DataSource\\sites.json";
            if (File.Exists(pathFile))
            {
                return JsonConvert.DeserializeObject<IEnumerable<Site>>(File.ReadAllText(pathFile));
            }
            else
            {
                throw new FileNotFoundException($"file not found at {pathFile}");
            }
        }
    }
}
