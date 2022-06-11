using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scrapping.Domain.Interfaces;
using Scrapping.Domain.Model;
using Scrapping.Interfaces;

namespace Scrapping.DomainServices.Site.Scan
{
    public class BaseScan : AbstractSite
    {
        public override SiteEnum SiteType => SiteEnum.Scan;
        private IReplace _replace;
        private IScrappingService _angleScrapService;
        private IDocument _documentService;

        public BaseScan(IReplace replace, IScrappingService angleScrapService, IDocument documentService)
        {
            _replace = replace;
            _angleScrapService = angleScrapService;
            _documentService = documentService;
        }

        public override async Task<List<Link>> GetAllLinks(int fromChapterNumber = 0)
        {
            var scrappingBag = await _angleScrapService.GetScrappingBagWithLinksForScan(SiteSelector.Url, fromChapterNumber, SiteSelector);

            return scrappingBag.Links;
        }

        public override async Task<string> GetMangaName()
        {
            var scrappingBag = await _angleScrapService.GetScrappingBagWithTextContent(SiteSelector.Url, SiteSelector.NameSelector);

            return _replace.Content(scrappingBag.TextContent, "", "[?|:|\"|\\n|/|/]");
        }

        protected override async Task<Result> InnerGenerateFileFromElements(Link link, string folderName)
        {
            var Result = new Result();
            StringBuilder text = new StringBuilder();

            try
            {
                var scrappingBag = await _angleScrapService.GetScrappingBagWithSource(link.Href, SiteSelector.ContentSelector);

                if (!String.IsNullOrEmpty(scrappingBag.Source))
                {
                    _documentService.DownloadNewPicture(folderName, link.Name, scrappingBag.Source, link.Chapter);
                }

            }
            catch (Exception ex)
            {
                Result.HasFailed(link, ex);
            }

            return Result;
        }

        public override IEnumerable<Link> RemoveLinksAlreadyDownload(List<Link> links, string folderName)
        {
            var paths = _documentService.GetAllFolders(folderName);
            if (paths.Count() > 0)
            {
                foreach (var path in paths)
                {
                    var chapterTitle = path.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries).Last();
                    var pathFiles = Directory.GetFiles(path);

                    foreach (var pathFile in pathFiles)
                    {
                        var fileTitle = pathFile.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries).Last().Replace(".jpg", "");
                        links.RemoveAll(l => l.Name == fileTitle && l.Chapter == chapterTitle);
                    }
                }
            }

            return links;
        }
    }
}
