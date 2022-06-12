using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Scrapping.Domain.Interfaces;
using Scrapping.Domain.Model;
using Scrapping.Interfaces;

namespace Scrapping.DomainServices.Site.Scan
{
    public class ScanJs : BaseScan
    {
        private IScrappingTechnical _scrappingTechnical;
        public override SiteEnum SiteType => SiteEnum.ScanJS;
        public ScanJs(IReplace replace, IScrappingService angleScrapService, IDocument documentService, IScrappingTechnical scrappingTechnical) : base(replace, angleScrapService, documentService)
        {
            _scrappingTechnical = scrappingTechnical;
        }

        public override async Task<List<Link>> GetAllLinks(int fromChapterNumber = 0)
        {
            List<Link> links = new();

            var chapterUrls = await _scrappingTechnical.GetUrls(SiteSelector.Url, SiteSelector.LinkSelector);
            foreach(var chapterUrl in chapterUrls)
            {
                var images = await _scrappingTechnical.GetDatasetsByIndex(chapterUrl, SiteSelector.PageSelector);
                foreach (var image in images)
                {
                    var chapter = Regex.Match(chapterUrl, "\\d+$").Value;
                    links.Add(new Link(image.Value, image.Key.ToString(), chapter));
                }
            }

            return links;
        }

        protected override async Task<Result> InnerGenerateFileFromElements(Link link, string folderName)
        {
            var Result = new Result();

            try
            {
                _documentService.DownloadNewPicture(folderName, link.Name, link.Href, link.Chapter);
            }
            catch (Exception ex)
            {
                Result.HasFailed(link, ex);
            }

            return Result;
        }

    }
}
