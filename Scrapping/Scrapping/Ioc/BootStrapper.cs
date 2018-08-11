using StructureMap;
namespace Scrapping.Ioc
{
    static public class Bootstrapper
    {
        public static IContainer ContainerTool
        {
            get
            {
                IContainer child = new Container();
                child.Configure(cfg =>
                {
                    cfg.IncludeRegistry<ServiceRegistry>();
                    cfg.IncludeRegistry<SiteRegistry>();
                });

                return child;
            }
        }
    }
}