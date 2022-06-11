using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Scrapping.Domain.Interfaces;
using Scrapping.Domain.Model;
using Scrapping.Interfaces;

namespace Scrapping.DomainServices.Site.Novel
{
    public class WuxiaWorld : BaseNovel
    {
        public override SiteEnum SiteType => SiteEnum.Wuxiaworld;
        private IReplace _replace;
        private IScrappingService _angleScrapService;

        public WuxiaWorld(IReplace replace, IScrappingService angleScrapService, IDocument documentService) : base(replace, angleScrapService, documentService)
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
            var scrappingBag = await _angleScrapService.GetScrappingBagWithLinks(SiteSelector.Url, SiteSelector.LinkSelector);

            return scrappingBag.Links.Skip(fromChapterNumber).ToList();
        }

        private List<Link> GetChapterLinks(int fromChapterNumber)
        {
            var links = new List<Link>();
            var nextChapterUrl = SiteSelector.Url;

            while (true)
            {
                var scrappingBag = _angleScrapService.GetScrappingBagWithLinks(nextChapterUrl, SiteSelector.NextChapterSelector).Result;
                var firstLink = scrappingBag.Links.FirstOrDefault();

                if (firstLink == null || links.Count > 0 && firstLink.Href == links.Last()?.Href)
                {
                    break;
                }

                nextChapterUrl = firstLink.Href;



                links.Add(firstLink);
            }

            return links;
        }
    }
}
