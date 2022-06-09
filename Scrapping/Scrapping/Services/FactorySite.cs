using Microsoft.Extensions.DependencyInjection;
using Scrapping.Domain.Interfaces;
using Scrapping.DomainServices.Site.Novel;
using Scrapping.DomainServices.Site.Scan;
using System.Collections.Generic;
using System.Linq;

namespace Scrapping.Site
{
    public class FactorySite
    {
        private readonly IEnumerable<ISite> _sites;
        public FactorySite(IEnumerable<ISite> sites)
        {
            _sites = sites;
        }

        public ISite GetSite(string siteTypeAsText) => _sites.Where(s => s.SiteType.ToString() == siteTypeAsText).First();

        public static void Configures(ref IServiceCollection services)
        {
            services.AddSingleton<ISite, BaseNovel>();
            services.AddSingleton<ISite, Gravitytales>();
            services.AddSingleton<ISite, NovelFull>();
            services.AddSingleton<ISite, WebNovel>();
            services.AddSingleton<ISite, WuxiaWorld>();

            services.AddSingleton<ISite, BaseScan>();
            services.AddSingleton<ISite, MangaLel>();
        }
    }
}
