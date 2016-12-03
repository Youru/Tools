using Scrapping.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scrapping
{
    public interface IUtilityService
    {
        void SetSite(Site site);
        void GenerateFileFromElements(Link link, string folderName);
        Task<List<Link>> GetAllLinks(string url, int fromChapterNumber);
        Task<string> GetMangaName(string url);
    }
}