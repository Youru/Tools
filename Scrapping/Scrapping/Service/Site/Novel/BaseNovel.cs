using AngleSharp;
using AngleSharp.Dom.Html;
using Scrapping.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrapping
{
    public class BaseNovel : AbstractSiteService
    {
        private IReplace _replace;
        private IAngleScrap _angleScrapService;
        private IDocument _documentService;

        public BaseNovel(IReplace replace, IAngleScrap angleScrapService, IDocument documentService)
        {
            _replace = replace;
            _angleScrapService = angleScrapService;
            _documentService = documentService;
        }

        public override async Task<List<Link>> GetAllLinks(string url, int fromChapterNumber)
        {

            IBrowsingContext context = _angleScrapService.GetContext();

            if (Site.linkMode == LinkModeEnum.index)
            {
                return await GetIndexLinks(url, fromChapterNumber, context);
            }
            else
            {
                return GetChapterLinks(url, fromChapterNumber, context);
            }
        }

        private async Task<List<Link>> GetIndexLinks(string url, int fromChapterNumber, IBrowsingContext context)
        {
            var elements = await _angleScrapService.GetElements(context, url, Site.LinkSelector);

            return elements.Select(e => new Link()
            {
                Href = e.GetAttribute("href"),
                Name = _replace.Content(((IHtmlAnchorElement)e).PathName, "", "[?|:|\"|\\n|/|/]")
            }).Skip(fromChapterNumber).ToList();
        }

        private List<Link> GetChapterLinks(string url, int fromChapterNumber, IBrowsingContext context)
        {
            var links = new List<Link>();
            var nextChapterUrl = url;

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

        public override async Task<string> GetMangaName(string url)
        {
            IBrowsingContext context = _angleScrapService.GetContext();
            var element = await _angleScrapService.GetElement(context, url, Site.NameSelector);

            return _replace.Content(element.TextContent, "", "[?|:|\"|\\n|/|/]");
        }

        protected override async Task InnerGenerateFileFromElements(Link link, string folderName)
        {
            IBrowsingContext context = _angleScrapService.GetContext();
            StringBuilder text = new StringBuilder();
            Console.WriteLine($"Trying to dl {link.Name} with url {link.Href}");
            try
            {
                int i = 0;
                while (i < 3)
                {
                    var elements = await _angleScrapService.GetElements(context, link.Href, Site.ContentSelector);
                    elements.Where(e => !IfElementContainsWrongPart(e)).ToList().ForEach(e => text.Append(e.OuterHtml));

                    if (text.Length > 0)
                    {

                        Console.WriteLine($"{link.Name} has been downloaded");
                        _documentService.FillNewDocument(folderName, link.Name, text);
                        break;
                    }
                    i++;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }

        }
    }
}
