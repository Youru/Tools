using ScrappingNewTest.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScrappingNewTest.Interfaces
{
    public interface ISite
    {
        void SetSite(Site site);
        void GenerateFileFromElements(Link link, string folderName);
        Task<List<Link>> GetAllLinks(int fromChapterNumber = 0);
        Task<string> GetMangaName();
        List<Link> RemoveLinksAlreadyDownload(List<Link> links, string folderName);
        List<Link> RemainingLinks { get; set; }
    }
}
