using Scrapping.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scrapping
{
    public interface ISite
    {
        void SetSite(Site site);
        void GenerateFileFromElements(Link link, string folderName);
        Task<List<Link>> GetAllLinks(string url, int fromChapterNumber);
        Task<string> GetMangaName(string url);
        List<Link> RemoveLinksAlreadyDownload(List<Link> links, string folderName);
    }
}