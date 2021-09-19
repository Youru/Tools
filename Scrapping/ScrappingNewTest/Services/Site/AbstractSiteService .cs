using System.Collections.Generic;
using System.Threading.Tasks;
using model = ScrappingNewTest.Model;
using AngleSharp.Dom;
using ScrappingNewTest.Interfaces;

namespace ScrappingNewTest.Services.Site
{
    public abstract class AbstractSiteService : ISite
    {
        protected model.Site Site { get; set; }
        public List<model.Link> RemainingLinks { get; set; }

        public void SetSite(model.Site site)
        {
            Site = site;
            RemainingLinks = new List<model.Link>();
        }

        public abstract Task<List<model.Link>> GetAllLinks(int fromChapterNumber = 0);

        public abstract Task<string> GetMangaName();

        protected abstract Task InnerGenerateFileFromElements(model.Link Link, string folderName);

        public abstract List<model.Link> RemoveLinksAlreadyDownload(List<model.Link> Links, string folderName);

        public void GenerateFileFromElements(model.Link Link, string folderName) => InnerGenerateFileFromElements(Link, folderName).Wait();


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
