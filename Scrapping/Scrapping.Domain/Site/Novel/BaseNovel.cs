using Scrapping.Interfaces;
using Scrapping.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;

namespace Scrapping.DomainServices.Site.Novel
{
    public class BaseNovel : AbstractSite
    {
        private IReplace _replace;
        private IAngleScrap _angleScrapService;
        private IDocument _documentService;

        public override SiteEnum SiteType => SiteEnum.Novel;

        public BaseNovel(IReplace replace, IAngleScrap angleScrapService, IDocument documentService)
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
            var elements = await _angleScrapService.GetElements(SiteSelector.Url, SiteSelector.LinkSelector);

            return elements.Select(e => new Link()
            {
                Href = e.GetAttribute("href"),
                Name = _replace.Content(((IHtmlAnchorElement)e).PathName, "", "[?|:|\"|\\n|/|/]")
            }).Skip(fromChapterNumber).ToList();
        }

        private List<Link> GetChapterLinks(int fromChapterNumber)
        {
            var links = new List<Link>();
            var nextChapterUrl = SiteSelector.Url;

            while (true)
            {
                var hrefList = _angleScrapService.GetElements(nextChapterUrl, SiteSelector.NextChapterSelector).Result;
                var element = hrefList.FirstOrDefault();

                if (element == null || links.Count > 0 && element.GetAttribute("href") == links.Last()?.Href)
                {
                    break;
                }

                nextChapterUrl = element.GetAttribute("href");



                links.Add(new Link
                {
                    Href = nextChapterUrl,
                    Name = _replace.Content(((IHtmlAnchorElement)element).PathName, "", "[?|:|\"|\\n|/|/]")
                });
            }

            return links;
        }

        public override async Task<string> GetMangaName()
        {
            var element = await _angleScrapService.GetElement(SiteSelector.Url, SiteSelector.NameSelector);

            return _replace.Content(element.TextContent, "", "[?|:|\"|\\n|/|/]");
        }

        protected override async Task<Result> InnerGenerateFileFromElements(Link link, string folderName)
        {
            var Result = new Result();
            StringBuilder text = new StringBuilder();
            try
            {
                int i = 0;
                while (i < 3)
                {
                    var elements = await _angleScrapService.GetElements(link.Href, SiteSelector.ContentSelector);
                    elements.Where(e => !IfElementContainsWrongPart(e)).ToList().ForEach(e => text.Append(e.OuterHtml));

                    if (text.Length > 0)
                    {
                        _documentService.FillNewDocument(folderName, link.Name, text);
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
