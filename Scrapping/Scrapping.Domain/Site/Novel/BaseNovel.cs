using Scrapping.Interfaces;
using Scrapping.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Scrapping.Domain.Interfaces;

namespace Scrapping.DomainServices.Site.Novel
{
    public class BaseNovel : AbstractSite
    {
        protected IReplace _replace;
        protected IScrappingService _angleScrapService;
        protected IDocument _documentService;

        public override SiteEnum SiteType => SiteEnum.Novel;

        public BaseNovel(IReplace replace, IScrappingService angleScrapService, IDocument documentService)
        {
            _replace = replace;
            _angleScrapService = angleScrapService;
            _documentService = documentService;
        }

        public override async Task<List<Link>> GetAllLinks(int fromChapterNumber = 0)
        {
            if (SiteSelector.linkMode == LinkModeEnum.index)
            {
                return await GetIndexLinks(fromChapterNumber);
            }
            else
            {
                return GetChapterLinks(fromChapterNumber);
            }
        }

        private async Task<List<Link>> GetIndexLinks(int fromChapterNumber)
        {
            var scrappingBag = await _angleScrapService.GetScrappingBagWithLinks(SiteSelector.Url, SiteSelector.LinkSelector);

            return scrappingBag.Links.Skip(fromChapterNumber).ToList();
        }

        private List<Link> GetChapterLinks(int fromChapterNumber)
        {
            var links = new List<Link>();
            var nextChapterUrl = SiteSelector.Url;

            while (true)
            {
                var scrappingBag = _angleScrapService.GetScrappingBagWithLink(nextChapterUrl, SiteSelector.NextChapterSelector).Result;
                var link = scrappingBag.Link;
                if (link == null || links.Count > 0 && link.Href == links.Last()?.Href)
                {
                    break;
                }
                nextChapterUrl = link.Href;
                links.Add(link);
            }

            return links;
        }

        public override async Task<string> GetMangaName()
        {
            var scrappingBag = await _angleScrapService.GetScrappingBagWithTextContent(SiteSelector.Url, SiteSelector.NameSelector);
            return _replace.Content(scrappingBag.TextContent, "", "[?|:|\"|\\n|/|/|-| ]");
        }

        protected override async Task<Result> InnerGenerateFileFromElements(Link link, string folderName)
        {
            var Result = new Result();
            try
            {
                int i = 0;
                while (i < 3)
                {
                    var scrappingBag = await _angleScrapService.GetScrappingBagWithChapterContent(link.Href, SiteSelector.ContentSelector, SiteSelector.WrongParts);
                    if (scrappingBag.ChapterContent.Length > 0)
                    {
                        _documentService.FillNewDocument(folderName, link.Name, scrappingBag.ChapterContent);
                        break;
                    }
                    i++;
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
            var paths = _documentService.GetAllFiles(folderName);
            if (paths.Length > 0)
            {
                foreach (var path in paths)
                {
                    var title = path.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries).Last().Replace(".html", "");
                    links.RemoveAll(l => l.Name == title);
                }
            }

            return links;
        }
    }
}
