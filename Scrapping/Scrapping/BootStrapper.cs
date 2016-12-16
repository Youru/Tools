using StructureMap;
namespace Scrapping
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
                    cfg.IncludeRegistry<BaseRegistry>();
                });

                return child;
            }
        }
    }
}