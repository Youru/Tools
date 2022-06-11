using Scrapping.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scrapping.Domain.Interfaces
{
    public interface ISite
    {
        SiteEnum SiteType { get; }
        void SetSiteSelector(SiteSelector site);
        Task<List<Link>> GetAllLinks(int fromChapterNumber = 0);
        Task<string> GetMangaName();
        IEnumerable<Link> RemoveLinksAlreadyDownload(List<Link> links, string folderName);
        Result GenerateFilesFromElements(IEnumerable<Link> link, string folderName);
        Result RetryDownloadLinks(string folderName, IEnumerable<Link> links);
    }
}
