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
            IProcessGenerationService process = Bootstrapper.ContainerTool.GetInstance<IProcessGenerationService>();
            return await process.Process(args);
        }
    }
}
