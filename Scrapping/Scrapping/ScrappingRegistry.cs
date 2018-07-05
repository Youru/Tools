using StructureMap;
using Scrapping.Service;

namespace Scrapping
{
    public class ScrappingRegistry : Registry
    {
        public ScrappingRegistry()
        {
            this.For<IReplace>().Use<Replace>();
            this.For<IAngleScrap>().Use<AngleScrap>();
            this.For<IDocument>().Use<Document>();
            this.For<IProcessGeneration>().Use<ProcessGeneration>();
            this.For<ISite>().Use<BaseScan>().Named("scan");
            this.For<ISite>().Use<BaseNovel>().Named("novel");
            this.For<ISite>().Use<Gravitytales>().Named("gravitytales");
            this.For<ISite>().Use<WuxiaWorld>().Named("wuxiaworld");
            this.For<ISite>().Use<MangaLel>().Named("mangalel");
            this.For<ISite>().Use<WebNovel>().Named("webnovel");

        }
    }
}
