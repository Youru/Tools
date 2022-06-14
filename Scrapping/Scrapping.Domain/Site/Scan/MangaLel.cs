using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using Scrapping.Domain.Model;
using Scrapping.Interfaces;
using Scrapping.Domain.Interfaces;

namespace Scrapping.DomainServices.Site.Scan
{
    public class MangaLel : BaseScan
    {
        public override SiteEnum SiteType => SiteEnum.Mangalel;
        private IScrappingTechnical _scrappingTechnical;

        public MangaLel(IReplace replace, IScrappingService angleScrapService, IDocument documentService, IScrappingTechnical scrappingTechnical) : base(replace, angleScrapService, documentService)
        {
            _scrappingTechnical = scrappingTechnical;
        }

        public override async Task<List<Link>> GetAllLinks(int fromChapterNumber = 0)
        {
            var scrappingBag = await _angleScrapService.GetScrappingBagWithLinks(SiteSelector.Url, SiteSelector.LinkSelector);

            return scrappingBag.Links.Skip(fromChapterNumber).ToList();
        }

        protected override async Task<Result> InnerGenerateFileFromElements(Link link, string folderName)
        {
            var Result = new Result();
            var chapterFolder = $"{folderName}\\{link.Name}";

            try
            {
                _documentService.CreateNewFolder(chapterFolder);
                var images = await _scrappingTechnical.GetDatasetsByIndex(link.Href, SiteSelector.ContentSelector);

                foreach (var image in images)
                {
                    _documentService.DownloadNewPicture(chapterFolder, $"{image.Key}", image.Value);
                }

            }
            catch (Exception ex)
            {
                Result.HasFailed(link, ex);
            }

            return Result;
        }
    }
}
