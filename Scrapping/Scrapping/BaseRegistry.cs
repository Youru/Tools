using StructureMap;
using Scrapping.Service;

namespace Scrapping
{
    public class BaseRegistry : Registry
    {
        public BaseRegistry()
        {
            this.For<IRegexService>().Use<RegexService>();
            this.For<IAngleScrapService>().Use<AngleScrapService>();
            this.For<IDocumentService>().Use<DocumentService>();
            this.For<IProcessGenerationService>().Use<ProcessGenerationService>();
            this.For<ISiteService>().Use<GenericScanSiteService>().Named("scan");
            this.For<ISiteService>().Use<GenericNovelSiteService>().Named("novel");
            this.For<ISiteService>().Use<GravitytalesNovelSiteService>().Named("gravitytales");
        }
    }
}
