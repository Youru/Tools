using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using Scrapping.Model;
using Scrapping.Service.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Scrapping
{
    public class GenericScanUtilityService : AbstractSiteUtilityService
    {
        //protected string ContentSelector;
        //protected string LinkSelector;
        //protected string NameSelector;
        //protected string[] WrongParts;
        //protected string PageSelector;
        //protected string ListPageSelector;
        //protected string ChapterNameSelector;

        //protected string PatternPageNumber;
        //protected string PatternChapterNumber;

        private IRegexService _regexService;
        private IAngleScrapService _angleScrapService;
        private IDocumentService _documentService;

        public GenericScanUtilityService(IRegexService regexService, IAngleScrapService angleScrapService, IDocumentService documentService)
        {
            _regexService = regexService;
            _angleScrapService = angleScrapService;
            _documentService = documentService;
        }

        public override async Task<List<Link>> GetAllLinks(string url, int fromChapterNumber)
        {
            var links = new List<Link>();
            var angleScrapService = new AngleScrapService();
            IBrowsingContext context = angleScrapService.GetContext();
            var elements = await angleScrapService.GetElements(context, url, Site.LinkSelector);

            foreach (var element in elements.Skip(fromChapterNumber))
            {
                var elem = element as IHtmlAnchorElement;
                var page = await angleScrapService.GetElement(context, elem.Href, Site.PageSelector);
                if (page != null)
                {
                    var pages = page.QuerySelectorAll(Site.ListPageSelector);
                    var chapterName = page.QuerySelector(Site.ChapterNameSelector);
                    pages.ToList().ForEach(p => links.Add(new Link() { Href = _regexService.ReplaceContentWithPreText(elem.Href, p.TextContent, Site.PatternPageNumber), Name = _regexService.ReplaceContentWithPostText(_regexService.ReplaceContent(chapterName.TextContent, "", "[?|:|\"|\\n|/|/]"), p.TextContent, Site.PatternChapterNumber) }));
                }
            }

            return links;
        }

        public override async Task<string> GetMangaName(string url)
        {
            var angleScrapService = new AngleScrapService();
            IBrowsingContext context = angleScrapService.GetContext();
            var element = await angleScrapService.GetElement(context, url, Site.NameSelector);

            return _regexService.ReplaceContent(element.TextContent, "", "[?|:|\"|\\n|/|/]");
        }

        protected override async Task InnerGenerateFileFromElements(Link link, string folderName)
        {
            IBrowsingContext context = _angleScrapService.GetContext();
            StringBuilder text = new StringBuilder();

            try
            {
                var element = (IHtmlImageElement)await _angleScrapService.GetElement(context, link.Href, Site.ContentSelector);

                if (!String.IsNullOrEmpty(element.Source))
                {
                    _documentService.DownloadNewPicture(folderName, link.Name, element.Source);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }
        }

        private bool IfElementContainsWrongPart(IElement element)
        {
            foreach (var wrongPart in Site.WrongParts)
            {
                if (element.InnerHtml.Contains(wrongPart))
                    return true;
            }
            return false;
        }
    }
}
