using System.Collections.Generic;
using System.Threading.Tasks;
using Scrapping.Model;
using AngleSharp;
using System.Linq;
using System;
using System.Text.RegularExpressions;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using System.Text;
using System.Net;
using System.Diagnostics;
using Scrapping.Service.Interface;

namespace Scrapping
{
    public class UtilityLectureEnLigneService : GenericScanUtilityService
    {
        //public UtilityLectureEnLigneService()
        //{
        //ContentSelector = "#image";
        //PageSelector = "#reader";
        //ListPageSelector = "#head:first-child .pages option";
        //LinkSelector = ".accueil a";
        //NameSelector = "#contenu h2";
        //ChapterNameSelector = "h3";

        //PatternPageNumber = "\\d+.html";
        //PatternChapterNumber = "page \\d+";
        //WrongParts = new string[] { };
        //}
        private IRegexService _regexService;

        public UtilityLectureEnLigneService(IRegexService regexService, IAngleScrapService angleScrapService, IDocumentService documentService) : base(regexService, angleScrapService, documentService)
        {
            _regexService = regexService;
        }

        public override async Task<List<Link>> GetAllLinks(string url, int fromChapterNumber)
        {
            var links = new List<Link>();
            var angleScrapService = new AngleScrapService();
            IBrowsingContext context = angleScrapService.GetContext();
            var elements = await angleScrapService.GetElements(context, url, Site.LinkSelector);

            foreach (var element in elements.Reverse().Skip(fromChapterNumber))
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
    }
}
