using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Scrapping.Domain.Interfaces;
using Scrapping.Domain.Model;
using Scrapping.Interfaces;

namespace Scrapping.DomainServices.Site.Scan
{
    public class Mangakakalot : BaseScan
    {

        private IScrappingTechnical _scrappingTechnical;
        public override SiteEnum SiteType => SiteEnum.Mangakakalot;
        public Mangakakalot(IReplace replace, IScrappingService angleScrapService, IDocument documentService, IScrappingTechnical scrappingTechnical) : base(replace, angleScrapService, documentService)
        {
            _scrappingTechnical = scrappingTechnical;
        }

        public override async Task<List<Link>> GetAllLinks(int fromChapterNumber = 0)
        {
            List<Link> links = new();

            var chapterSummaryUrl = _replace.Content(SiteSelector.Url, "to/ajax/manga/list-chapter-volume?id=", "to\\/(.+)-");
            var chapterUrls = await _scrappingTechnical.GetUrls(chapterSummaryUrl, SiteSelector.PageSelector);

            foreach (var chapterUrl in chapterUrls)
            {
                var dataset = await _scrappingTechnical.GetDataset(chapterUrl, "#reading");
                var readingId = dataset.GetValueOrDefault("reading-id");
                var chapterImageUrl = $"https://{SiteSelector.BaseUrl.Host}/ajax/manga/images?id={readingId}&type=chap";
                var images = await _scrappingTechnical.GetDatasetsByIndex(chapterImageUrl, SiteSelector.LinkSelector);

                foreach (var image in images)
                {
                    var chapter = Regex.Match(chapterUrl, "(chapter-\\d+)$").Value;
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
