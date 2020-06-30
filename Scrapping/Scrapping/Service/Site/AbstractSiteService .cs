using System.Collections.Generic;
using System.Threading.Tasks;
using Scrapping.Model;
using AngleSharp.Dom;

namespace Scrapping
{
    public abstract class AbstractSiteService : ISite
    {
        protected Site Site { get; set; }
        public List<Link> RemainingLinks { get; set; }

        public void SetSite(Site site)
        {
            Site = site;
            RemainingLinks = new List<Link>();
        }

        public abstract Task<List<Link>> GetAllLinks(string url, int fromChapterNumber);

        public abstract Task<string> GetMangaName(string url);

        protected abstract Task InnerGenerateFileFromElements(Link link, string folderName);

        public abstract List<Link> RemoveLinksAlreadyDownload(List<Link> links, string folderName);

        public void GenerateFileFromElements(Link link, string folderName)
        {
            InnerGenerateFileFromElements(link, folderName).Wait();
        }

        protected bool IfElementContainsWrongPart(IElement element)
        {
            foreach (var wrongPart in Site.WrongParts)
            {
                if (element.InnerHtml.Contains(wrongPart))
                    return true;
            }
            return false;
        }
    }
}
