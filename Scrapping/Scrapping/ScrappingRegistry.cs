using StructureMap;
using Scrapping.Service;

namespace Scrapping
{
    public class ScrappingRegistry : Registry
    {
        public ScrappingRegistry()
        {
            this.For<IRegexService>().Use<RegexService>();
            this.For<IAngleScrapService>().Use<AngleScrapService>();
            this.For<IDocumentService>().Use<DocumentService>();
            this.For<IProcessGenerationService>().Use<ProcessGenerationService>();
            this.For<ISiteService>().Use<BaseScan>().Named("scan");
            this.For<ISiteService>().Use<BaseNovel>().Named("novel");
            this.For<ISiteService>().Use<Gravitytales>().Named("gravitytales");
            this.For<ISiteService>().Use<WuxiaWorld>().Named("wuxiaworld");
            this.For<ISiteService>().Use<MangaLel>().Named("mangalel");
            this.For<ISiteService>().Use<WebNovel>().Named("webnovel");

        }
    }
}
