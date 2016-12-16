using System.Collections.Generic;
using System.Threading.Tasks;
using Scrapping.Model;
using AngleSharp.Dom;

namespace Scrapping
{
    public abstract class AbstractSiteService : ISiteService
    {
        protected Site Site { get; set; }

        public void SetSite(Site site)
        {
            Site = site;
        }

        public abstract Task<List<Link>> GetAllLinks(string url, int fromChapterNumber);

        public abstract Task<string> GetMangaName(string url);

        public void GenerateFileFromElements(Link link, string folderName)
        {
            InnerGenerateFileFromElements(link, folderName).Wait();
        }

        protected abstract Task InnerGenerateFileFromElements(Link link, string folderName);
        
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
