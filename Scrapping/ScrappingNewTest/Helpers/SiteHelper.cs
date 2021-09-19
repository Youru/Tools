using ScrappingNewTest.Model;
using System;
using System.Linq;

namespace ScrappingNewTest.Helpers
{
    public static class SiteHelper
    {
        public static Site GetSiteByUrl(string url)
        {
            var sites = DataHelper.GetSites();
            var site = sites.FirstOrDefault(s => url.Contains(s.Resolve));
            site.Url = url;

            if (site.HasError())
                throw new Exception("Missing fields for site");

            return site;
        }
    }
}
