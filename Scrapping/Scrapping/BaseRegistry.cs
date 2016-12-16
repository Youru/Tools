using StructureMap;
using Scrapping.Service.Interface;
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
            this.For<IProcessGeneration>().Use<ProcessGeneration>();
            this.For<ISiteService>().Use<GenericScanSiteService>().Named("scan");
            this.For<ISiteService>().Use<GenericNovelSiteService>().Named("novel");
            this.For<ISiteService>().Use<LectureEnLigneSiteService>().Named("lec");
        }
    }
}
