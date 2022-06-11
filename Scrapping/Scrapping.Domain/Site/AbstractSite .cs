using System.Collections.Generic;
using System.Threading.Tasks;
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


        public Result GenerateFilesFromElements(IEnumerable<Link> links, string folderName)
        {
            Result result = new Result();
            List<Task<Result>> resultTasks = new List<Task<Result>>();

            Parallel.ForEach(links, currentLink =>
            {
                var result = InnerGenerateFileFromElements(currentLink, folderName);
                resultTasks.Add(result);
            });

            foreach (var task in resultTasks)
            {
                if (!task.Result.HasSuceed)
                {
                    task.Result.LinkExceptions.ForEach(le => result.HasFailed(le.Link, le.Exception));
                }
            }

            return result;
        }

        public Result RetryDownloadLinks(string folderName, IEnumerable<Link> links)
        {
            var result = new Result();

            for (int i = 0; i < 5; i++)
            {
                if (!links.Any())
                    return result;
                result = GenerateFilesFromElements(links, folderName);
                links = result.LinkExceptions.Select(le => le.Link);
            }

            return result;
        }
    }
}
