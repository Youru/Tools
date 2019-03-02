using AngleSharp;
using AngleSharp.Dom;
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
    public class NovelFull : BaseNovel
    {
        private IReplace _replace;
        private IAngleScrap _angleScrapService;
        private IDocument _documentService;

        public NovelFull(IReplace replace, IAngleScrap angleScrapService, IDocument documentService) : base(replace, angleScrapService, documentService)
        {
            _replace = replace;
            _angleScrapService = angleScrapService;
            _documentService = documentService;
        }

        public override async Task<List<Link>> GetAllLinks(string url, int fromChapterNumber)
        {
            IBrowsingContext context = _angleScrapService.GetContext();
            var lastPage = false;
            var listLink = new List<Link>();
            var pageUrl = url;

            while (!lastPage)
            {
                var elements = await _angleScrapService.GetElements(context, pageUrl, Site.LinkSelector);

                listLink.AddRange(CreateListLinks(elements));

                pageUrl = await GetNextPage(context, pageUrl, url);
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
            Name = _replace.Content(((IHtmlAnchorElement)e).Title, "-", "[?|:|\"|\\n|/|/]")
        }).ToList();
    }
}
