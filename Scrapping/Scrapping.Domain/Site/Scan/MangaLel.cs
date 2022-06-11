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
        private IReplace _replace;
        private IScrappingService _angleScrapService;
        private IDocument _documentService;

        public MangaLel(IReplace replace, IScrappingService angleScrapService, IDocument documentService) : base(replace, angleScrapService, documentService)
        {
            _replace = replace;
            _angleScrapService = angleScrapService;
            _documentService = documentService;
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
                var scrappingBag = await _angleScrapService.GetScrappingBagWithSourceByDataset(link.Href, SiteSelector.ContentSelector);

                for (int i = 0; i <= scrappingBag.Sources.Count; i++)
                {
                    _documentService.DownloadNewPicture(chapterFolder, $"{i + 1}", scrappingBag.Sources[i]);
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
