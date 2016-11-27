using Newtonsoft.Json;
using Scrapping.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

namespace Scrapping
{
    public static class UtilityServiceResolver
    {

        public static bool TryResolveUrl(string url, ref AbstractUtilityService utilityService, List<Site> sites = null)
        {
            utilityService = ResolveUrl(url);

            if (utilityService == null && sites != null && sites.Count > 0)
            {
                utilityService = ResolveUrlWithAdditionalSite(sites, url);
            }

            return utilityService != null;
        }

        public static AbstractUtilityService ResolveUrl(string url)
        {
            if (url.Contains("wuxia"))
                return new UtilityWuxiaService();
            else if (url.Contains("translationnations.com"))
                return new UtilityTranslationService();
            else if (url.Contains("royalroadweed.blogspot.fr/"))
                return new UtilityRoyalroadWeedService();

            return null;
        }

        private static AbstractUtilityService ResolveUrlWithAdditionalSite(List<Site> sites, string url)
        {
            foreach (var site in sites)
            {
                if (url.Contains(site.Resolve))
                {
                    return new AdditionnalSiteUtilityService(site.ContentSelector, site.LinkSelector, site.NameSelector, site.WrongParts);
                }
            }

            return null;
        }


    }
}
