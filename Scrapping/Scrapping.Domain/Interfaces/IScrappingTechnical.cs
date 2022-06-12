using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scrapping.Domain.Interfaces
{
    public interface IScrappingTechnical
    {
        Task<Dictionary<string, string>> GetDataset(string url, string selector);
        Task<Dictionary<int, string>> GetDatasetsByIndex(string url, string selector);
        Task<string[]> GetUrls(string url, string selector);
    }
}