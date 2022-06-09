using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Scrapping.Domain.Model;
using AngleSharp.Html.Dom;
using AngleSharp.Dom;
using Scrapping.Interfaces;

namespace Scrapping.DomainServices.Site.Novel
{
    public class NovelFull : BaseNovel
    {
        public override SiteEnum SiteType => SiteEnum.Novelfull;
        private IReplace _replace;
        private IAngleScrap _angleScrapService;

        public NovelFull(IReplace replace, IAngleScrap angleScrapService, Interfaces.IDocument documentService) : base(replace, angleScrapService, documentService)
        {
            _replace = replace;
            _angleScrapService = angleScrapService;
        }

        public override async Task<List<Link>> GetAllLinks(int fromChapterNumber = 0)
        {
            var lastPage = false;
            var listLink = new List<Link>();
            var pageUrl = SiteSelector.Url;

            while (!lastPage)
            {
                var elements = await _angleScrapService.GetElements(pageUrl, SiteSelector.LinkSelector);

                listLink.AddRange(CreateListLinks(elements));

                pageUrl = await GetNextPage(pageUrl, SiteSelector.Url);
                lastPage = pageUrl == null;
            }

            return listLink;
        }

        private async Task<string> GetNextPage(string url, string rootUrl)
        {
            var pageSelector = await _angleScrapService.GetElements(url, SiteSelector.PageSelector);
            string nextUrl = null;

            if (pageSelector.Length > 0)
            {
                var numberPageSelection = await _angleScrapService.GetElements(url, SiteSelector.ListPageSelector);
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
