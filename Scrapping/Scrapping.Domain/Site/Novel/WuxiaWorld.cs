using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using Scrapping.Domain.Model;
using Scrapping.Interfaces;

namespace Scrapping.DomainServices.Site.Novel
{
    public class WuxiaWorld : BaseNovel
    {
        public override SiteEnum SiteType => SiteEnum.Wuxiaworld;
        private IReplace _replace;
        private IAngleScrap _angleScrapService;

        public WuxiaWorld(IReplace replace, IAngleScrap angleScrapService, IDocument documentService) : base(replace, angleScrapService, documentService)
        {
            _replace = replace;
            _angleScrapService = angleScrapService;
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
                Href = new Uri(SiteSelector.BaseUrl, e.GetAttribute("href")).ToString(),
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
    }
}
