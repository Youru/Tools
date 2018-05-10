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
    public class WuxiaWorld : BaseNovel
    {
        private IRegexService _regexService;
        private IAngleScrapService _angleScrapService;
        private IDocumentService _documentService;

        public WuxiaWorld(IRegexService regexService, IAngleScrapService angleScrapService, IDocumentService documentService):base(regexService,  angleScrapService,  documentService)
        {
            _regexService = regexService;
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
                Href = new Uri(Site.BaseUrl,e.GetAttribute("href")).ToString(),
                Name = _regexService.ReplaceContent(((IHtmlAnchorElement)e).PathName, "", "[?|:|\"|\\n|/|/]")
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
                    Name = _regexService.ReplaceContent(((IHtmlAnchorElement)element).PathName, "", "[?|:|\"|\\n|/|/]")
                });
            }

            return links;
        }
    }
}
