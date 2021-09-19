using AngleSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScrappingNewTest.Interfaces;
using ScrappingNewTest.Model;
using AngleSharp.Html.Dom;
using Microsoft.Extensions.Logging;

namespace ScrappingNewTest.Services.Site.Novel
{
    public class BaseNovel : AbstractSiteService
    {
        private IReplace _replace;
        private IAngleScrap _angleScrapService;
        private IDocument _documentService;
        private readonly ILogger<BaseNovel> _logger;

        public BaseNovel(IReplace replace, IAngleScrap angleScrapService, IDocument documentService, ILogger<BaseNovel> logger)
        {
            _replace = replace;
            _angleScrapService = angleScrapService;
            _documentService = documentService;
            _logger = logger;
        }

        public override async Task<List<Link>> GetAllLinks(int fromChapterNumber = 0)
        {

            IBrowsingContext context = _angleScrapService.GetContext();

            if (Site.linkMode == LinkModeEnum.index)
            {
                return await GetIndexLinks(fromChapterNumber, context);
            }
            else
            {
                return GetChapterLinks(fromChapterNumber, context);
            }
        }

        private async Task<List<Link>> GetIndexLinks(int fromChapterNumber, IBrowsingContext context)
        {
            var elements = await _angleScrapService.GetElements(context, Site.Url, Site.LinkSelector);

            return elements.Select(e => new Link()
            {
                Href = e.GetAttribute("href"),
                Name = _replace.Content(((IHtmlAnchorElement)e).PathName, "", "[?|:|\"|\\n|/|/]")
            }).Skip(fromChapterNumber).ToList();
        }

        private List<Link> GetChapterLinks(int fromChapterNumber, IBrowsingContext context)
        {
            var links = new List<Link>();
            var nextChapterUrl = Site.Url;

            while (true)
            {
                var hrefList = _angleScrapService.GetElements(context, nextChapterUrl, Site.NextChapterSelector).Result;
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
            IBrowsingContext context = _angleScrapService.GetContext();
            var element = await _angleScrapService.GetElement(context, Site.Url, Site.NameSelector);

            return _replace.Content(element.TextContent, "", "[?|:|\"|\\n|/|/]");
        }

        protected override async Task InnerGenerateFileFromElements(Link link, string folderName)
        {
            IBrowsingContext context = _angleScrapService.GetContext();
            StringBuilder text = new StringBuilder();
            _logger.LogInformation($"Trying to dl {link.Name} with url {link.Href}");
            try
            {
                int i = 0;
                while (i < 3)
                {
                    var elements = await _angleScrapService.GetElements(context, link.Href, Site.ContentSelector);
                    elements.Where(e => !IfElementContainsWrongPart(e)).ToList().ForEach(e => text.Append(e.OuterHtml));

                    if (text.Length > 0)
                    {

                        _logger.LogInformation($"{link.Name} has been downloaded");
                        _documentService.FillNewDocument(folderName, link.Name, text);
                        break;
                    }
                    i++;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

        }

        public override List<Link> RemoveLinksAlreadyDownload(List<Link> links, string folderName)
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
