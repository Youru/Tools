using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Scrapping.Domain.Interfaces;
using Scrapping.Domain.Model;
using Scrapping.Interfaces;

namespace Scrapping.DomainServices.Site.Novel
{
    public class NovelPub : BaseNovel
    {
        public override SiteEnum SiteType => SiteEnum.Novelpub;

        public NovelPub(IReplace replace, IScrappingService angleScrapService, IDocument documentService) : base(replace, angleScrapService, documentService)
        {
        }

        public override async Task<List<Link>> GetAllLinks(int fromChapterNumber = 0)
        {
            var lastPage = false;
            var listLink = new List<Link>();
            var pageUrl = SiteSelector.Url;

            while (!lastPage)
            {
                var scrappingBag = await _angleScrapService.GetScrappingBagWithTitleLinks(pageUrl, SiteSelector.LinkSelector);

                var novelPubLinks = scrappingBag.Links.Select(l => new Link(
                    $"{SiteSelector.BaseUrl.Scheme}://{SiteSelector.BaseUrl.Host}{l.Href}",
                    _replace.Content(l.Name, "", "[?|>|:|\"|\\n|/|/|-| ]"),
                    l.Href.Split("/").Last()
                    )
                );

                listLink.AddRange(novelPubLinks);

                var scrappingBagWithUrl = await _angleScrapService.GetScrappingBagWithLink(pageUrl, SiteSelector.PageSelector);
                pageUrl = scrappingBagWithUrl.Link != null ?
                    $"{SiteSelector.BaseUrl.Scheme}://{SiteSelector.BaseUrl.Host}{scrappingBagWithUrl.Link.Href}" : null;
                lastPage = pageUrl == null;
            }

            return listLink;
        }
    }
}
