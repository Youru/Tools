using System.Collections.Generic;
using System.Threading.Tasks;
using Scrapping.Model;
using AngleSharp;
using System.Linq;
using AngleSharp.Dom.Html;

namespace Scrapping
{
    public class LectureEnLigneSiteService : GenericScanSiteService
    {
        private IRegexService _regexService;
        private IAngleScrapService _angleScrapService;

        public LectureEnLigneSiteService(IRegexService regexService, IAngleScrapService angleScrapService, IDocumentService documentService) : base(regexService, angleScrapService, documentService)
        {
            _regexService = regexService;
            _angleScrapService = angleScrapService;
        }

        public override async Task<List<Link>> GetAllLinks(string url, int fromChapterNumber)
        {
            var links = new List<Link>();
            IBrowsingContext context = _angleScrapService.GetContext();
            var elements = await _angleScrapService.GetElements(context, url, Site.LinkSelector);

            foreach (var element in elements.Reverse().Skip(fromChapterNumber))
            {
                var elem = element as IHtmlAnchorElement;
                var page = await _angleScrapService.GetElement(context, elem.Href, Site.PageSelector);
                if (page != null)
                {
                    var pages = page.QuerySelectorAll(Site.ListPageSelector);
                    var chapterName = page.QuerySelector(Site.ChapterNameSelector);
                    pages.ToList().ForEach(p => links.Add(new Link() { Href = _regexService.ReplaceContentWithPreText(elem.Href, p.TextContent, Site.PatternPageNumber), Name = _regexService.ReplaceContentWithPostText(_regexService.ReplaceContent(chapterName.TextContent, "", "[?|:|\"|\\n|/|/]"), p.TextContent, Site.PatternChapterNumber) }));
                }
            }

            return links;
        }
    }
}
