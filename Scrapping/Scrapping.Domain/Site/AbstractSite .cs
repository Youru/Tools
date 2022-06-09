using System.Collections.Generic;
using System.Threading.Tasks;
using AngleSharp.Dom;
using System.Linq;
using Scrapping.Domain.Interfaces;
using Scrapping.Domain.Model;

namespace Scrapping.DomainServices.Site
{
    public abstract class AbstractSite : ISite
    {
        protected SiteSelector SiteSelector { get; set; }
        public abstract SiteEnum SiteType { get; }

        public void SetSite(SiteSelector site)
        {
            SiteSelector = site;
        }

        public abstract Task<List<Link>> GetAllLinks(int fromChapterNumber = 0);

        public abstract Task<string> GetMangaName();

        protected abstract Task<Result> InnerGenerateFileFromElements(Link Link, string folderName);

        public abstract IEnumerable<Link> RemoveLinksAlreadyDownload(List<Link> Links, string folderName);


        public async Task<IEnumerable<Link>> GenerateFilesFromElements(IEnumerable<Link> links, string folderName)
        {
            var remainingLink = new List<Link>();
            List<Task<Result>> resultTasks = new List<Task<Result>>();

            Parallel.ForEach(links, currentLink =>
            {
                var result = InnerGenerateFileFromElements(currentLink, folderName);
                resultTasks.Add(result);
            });

            foreach (var task in resultTasks)
            {
                if (!task.Result.IsSuceed) remainingLink.Add(task.Result.Link);
            }

            return remainingLink;
        }

        public async Task<IEnumerable<Link>> RetryDownloadLinks(string folderName, IEnumerable<Link> links)
        {
            for (int i = 0; i < 5; i++)
            {
                if (!links.Any())
                    return links;
                links = await GenerateFilesFromElements(links, folderName);
            }

            return links;
        }

        protected bool IfElementContainsWrongPart(IElement element)
        {
            foreach (var wrongPart in SiteSelector.WrongParts)
            {
                if (element.InnerHtml.Contains(wrongPart))
                    return true;
            }

            return false;
        }
    }
}
