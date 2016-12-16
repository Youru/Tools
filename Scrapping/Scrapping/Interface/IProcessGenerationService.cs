using System.Threading.Tasks;

namespace Scrapping
{
    public interface IProcessGenerationService
    {
        Task<int> Process(string[] args);
    }
}