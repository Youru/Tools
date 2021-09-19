using AngleSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScrappingNewTest.Interfaces;
using ScrappingNewTest.Model;
using AngleSharp.Html.Dom;
using AngleSharp.Dom;
using Microsoft.Extensions.Logging;

namespace ScrappingNewTest.Services.Site.Novel
{
    public class NovelFull : BaseNovel
    {
        private IReplace _replace;
        private IAngleScrap _angleScrapService;

        public NovelFull(IReplace replace, IAngleScrap angleScrapService, Interfaces.IDocument documentService, ILogger<NovelFull> logger) : base(replace, angleScrapService, documentService, logger)
        {
            _replace = replace;
            _angleScrapService = angleScrapService;
        }

        public override async Task<List<Link>> GetAllLinks(int fromChapterNumber = 0)
        {
            IBrowsingContext context = _angleScrapService.GetContext();
            var lastPage = false;
            var listLink = new List<Link>();
            var pageUrl = Site.Url;

            while (!lastPage)
            {
                var elements = await _angleScrapService.GetElements(context, pageUrl, Site.LinkSelector);

                listLink.AddRange(CreateListLinks(elements));

                pageUrl = await GetNextPage(context, pageUrl, Site.Url);
                lastPage = pageUrl == null;
            }

            return listLink;
        }

        private async Task<string> GetNextPage(IBrowsingContext context, string url, string rootUrl)
        {
            var pageSelector = await _angleScrapService.GetElements(context, url, Site.PageSelector);
            string nextUrl = null;

            if (pageSelector.Length > 0)
            {
                var numberPageSelection = await _angleScrapService.GetElements(context, url, Site.ListPageSelector);
                nextUrl = $"{rootUrl}?page={Int32.Parse(numberPageSelection[0].TextContent) + 1}";
            }

            return nextUrl;
        }

        private List<Link> CreateListLinks(IHtmlCollection<IElement> elements)
        => elements.Select(e => new Link()
        {
            Href = ((IHtmlAnchorElement)e).Href,
            Name = _replace.Content(((IHtmlAnchorElement)e).Title, "-", "[*|?|:|\"|\\n|/|/]")
        }).ToList();
    }
}
