using Scrapping.Domain.Model;
using System;
using System.Linq;

namespace Scrapping.Helpers
{
    public static class SiteHelper
    {
        public static SiteSelector GetSiteByUrl(string url)
        {
            var siteSelectors = DataHelper.GetSiteSelectors();
            var siteSelector = siteSelectors.FirstOrDefault(s => url.Contains(s.Resolve));
            siteSelector.Url = url;

            var result = siteSelector.Validate();

            if (result.Any())
                throw new Exception($"Missing fields for site : {string.Join(" | ", result)}");

            return siteSelector;
        }
    }
}
