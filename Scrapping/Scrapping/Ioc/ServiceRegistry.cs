using StructureMap;
using Scrapping.Service;

namespace Scrapping.Ioc
{
    public class ServiceRegistry : Registry
    {
        public ServiceRegistry()
        {
            this.For<IReplace>().Use<Replace>();
            this.For<IAngleScrap>().Use<AngleScrap>();
            this.For<IDocument>().Use<Document>();
            this.For<IProcessGeneration>().Use<ProcessGeneration>();
        }
    }
}
