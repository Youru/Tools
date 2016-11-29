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

        public static bool TryResolveUrl(string url, ref IUtilityService utilityService, List<Site> sites = null)
        {
            if (sites != null && sites.Count > 0)
            {
                utilityService = ResolveUrlWithAdditionalSite(sites, url);
            }

            if (utilityService == null)
            {
                utilityService = ResolveUrl(url);
            }
            
            return utilityService != null;
        }

        public static IUtilityService ResolveUrl(string url)
        {
            if (url.Contains("wuxia"))
                return new UtilityWuxiaService();
            else if (url.Contains("translationnations.com"))
                return new UtilityTranslationService();
            else if (url.Contains("royalroadweed.blogspot.fr/"))
                return new UtilityRoyalroadWeedService();
            else if (url.Contains("lecture-en-ligne.com"))
                return new UtilityLectureEnLigneService();
            else if (url.Contains("mangareader.net"))
                return new UtilityMangaReaderService();
            return null;
        }

        private static IUtilityService ResolveUrlWithAdditionalSite(List<Site> sites, string url)
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
