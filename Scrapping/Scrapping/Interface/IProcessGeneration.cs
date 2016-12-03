using System.Threading.Tasks;

namespace Scrapping
{
    public interface IProcessGeneration
    {
        Task<int> Process(string[] args);
    }
}