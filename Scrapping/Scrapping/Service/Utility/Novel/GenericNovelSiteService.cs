using AngleSharp;
using AngleSharp.Dom.Html;
using Scrapping.Model;
using Scrapping.Service.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrapping
{
    public class GenericNovelSiteService : AbstractSiteService
    {
        private IRegexService _regexService;
        private IAngleScrapService _angleScrapService;
        private IDocumentService _documentService;

        public GenericNovelSiteService(IRegexService regexService, IAngleScrapService angleScrapService, IDocumentService documentService)
        {
            _regexService = regexService;
            _angleScrapService = angleScrapService;
            _documentService = documentService;
        }

        public override async Task<List<Link>> GetAllLinks(string url, int fromChapterNumber)
        {

            IBrowsingContext context = _angleScrapService.GetContext();
            var elements = await _angleScrapService.GetElements(context, url, Site.LinkSelector);

            return elements.Select(e => new Link()
            {
                Href = e.GetAttribute("href"),
                Name = _regexService.ReplaceContent(((IHtmlAnchorElement)e).PathName, "", "[?|:|\"|\\n|/|/]")
            }).Skip(fromChapterNumber).ToList();
        }

        public override async Task<string> GetMangaName(string url)
        {
            IBrowsingContext context = _angleScrapService.GetContext();
            var element = await _angleScrapService.GetElement(context, url, Site.NameSelector);

            return _regexService.ReplaceContent(element.TextContent, "", "[?|:|\"|\\n|/|/]");
        }

        protected override async Task InnerGenerateFileFromElements(Link link, string folderName)
        {
            IBrowsingContext context = _angleScrapService.GetContext();
            StringBuilder text = new StringBuilder();

            try
            {
                var elements = await _angleScrapService.GetElements(context, link.Href, Site.ContentSelector);
                elements.Where(e => !IfElementContainsWrongPart(e)).ToList().ForEach(e => text.Append(e.OuterHtml));

                if (text.Length > 0)
                {
                    _documentService.FillNewDocument(folderName, link.Name, text);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }

        }
    }
}
