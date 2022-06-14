using System.Collections.Generic;
using System.Threading.Tasks;
using Scrapping.Domain.Model;

namespace Scrapping.Domain.Interfaces
{
    public interface IScrappingService
    {
        Task<ScrappingBag> GetScrappingBagWithTextContent(string url, string selector);
        Task<ScrappingBag> GetScrappingBagWithSource(string url, string selector);
        Task<ScrappingBag> GetScrappingBagWithChapterContent(string url, string selector, string[] wrongParts);
        Task<ScrappingBag> GetScrappingBagWithLink(string url, string selector);
        Task<List<ScrappingBag>> GetScrappingBagWithUrls(string url, string selector);
        Task<ScrappingBag> GetScrappingBagWithNextPageUrl(string url, SiteSelector selector);
        Task<ScrappingBag> GetScrappingBagWithLinks(string url, string selector);
        Task<ScrappingBag> GetScrappingBagWithLinksForScan(string url, int fromChapterNumber, SiteSelector siteSelector);
        Task<ScrappingBag> GetScrappingBagWithTitleLinks(string url, string selector);
    }
}