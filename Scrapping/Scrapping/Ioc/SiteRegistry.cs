using StructureMap;
using Scrapping.Service;

namespace Scrapping.Ioc
{
    public class SiteRegistry : Registry
    {
        public SiteRegistry()
        {
            this.For<ISite>().Use<BaseScan>().Named("scan");
            this.For<ISite>().Use<BaseNovel>().Named("novel");
            this.For<ISite>().Use<Gravitytales>().Named("gravitytales");
            this.For<ISite>().Use<WuxiaWorld>().Named("wuxiaworld");
            this.For<ISite>().Use<MangaLel>().Named("mangalel");
            this.For<ISite>().Use<WebNovel>().Named("webnovel");
            this.For<ISite>().Use<NovelFull>().Named("novelfull");
        }
    }
}
