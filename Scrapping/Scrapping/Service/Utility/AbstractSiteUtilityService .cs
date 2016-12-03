using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Scrapping.Model;

namespace Scrapping
{
    public abstract class AbstractSiteUtilityService : IUtilityService
    {
        protected Site Site { get; set; }
        public void GenerateFileFromElements(Link link, string folderName)
        {
            InnerGenerateFileFromElements(link, folderName).Wait();
        }

        protected abstract Task InnerGenerateFileFromElements(Link link, string folderName);

        public abstract Task<List<Link>> GetAllLinks(string url, int fromChapterNumber);

        public abstract Task<string> GetMangaName(string url);

        public void SetSite(Site site)
        {
            Site = site;
        }
    }
}
