using StructureMap;
using Scrapping.Service.Interface;
using Scrapping.Service;
using Scrapping;
using Scrapping.Model;

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
            this.For<IUtilityService>().Use<GenericScanUtilityService>().Named("scan");
            this.For<IUtilityService>().Use<GenericNovelUtilityService>().Named("novel");
            this.For<IUtilityService>().Use<UtilityLectureEnLigneService>().Named("lec");
        }
    }
}
