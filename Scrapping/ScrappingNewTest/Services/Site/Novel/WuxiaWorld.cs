using AngleSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScrappingNewTest.Interfaces;
using ScrappingNewTest.Model;
using AngleSharp.Html.Dom;
using Microsoft.Extensions.Logging;

namespace ScrappingNewTest.Services.Site.Novel
{
    public class WuxiaWorld : BaseNovel
    {
        private IReplace _replace;
        private IAngleScrap _angleScrapService;

        public WuxiaWorld(IReplace replace, IAngleScrap angleScrapService, IDocument documentService, ILogger<WuxiaWorld> logger) : base(replace, angleScrapService, documentService, logger)
        {
            _replace = replace;
            _angleScrapService = angleScrapService;
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
                Href = new Uri(Site.BaseUrl, e.GetAttribute("href")).ToString(),
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
    }
}
