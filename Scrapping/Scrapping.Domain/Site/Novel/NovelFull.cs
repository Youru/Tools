﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Scrapping.Domain.Interfaces;
using Scrapping.Domain.Model;
using Scrapping.Interfaces;

namespace Scrapping.DomainServices.Site.Novel
{
    public class NovelFull : BaseNovel
    {
        public override SiteEnum SiteType => SiteEnum.Novelfull;

        public NovelFull(IReplace replace, IScrappingService angleScrapService, IDocument documentService) : base(replace, angleScrapService, documentService)
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

                listLink.AddRange(scrappingBag.Links);

                var scrappingBagWithUrl = await _angleScrapService.GetScrappingBagWithNextPageUrl(pageUrl, SiteSelector);
                pageUrl = scrappingBagWithUrl.Link.Href;
                lastPage = pageUrl == null;
            }

            return listLink;
        }
    }
}
