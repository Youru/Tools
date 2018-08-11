using Scrapping.Ioc;
using System.Threading.Tasks;

namespace Scrapping
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Execute(args).Wait();
        }

        public static async Task<int> Execute(string[] args)
        {
            IProcessGeneration process = Bootstrapper.ContainerTool.GetInstance<IProcessGeneration>();
            return await process.Process(args);
        }
    }
}
