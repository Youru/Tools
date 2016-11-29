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

namespace Scrapping
{
    public class UtilityLectureEnLigneService : AbstractScanUtilityService
    {
        public UtilityLectureEnLigneService()
        {
            ContentSelector = "#image";
            PageSelector = "#reader";
            ListPageSelector = "#head:first-child .pages option";
            LinkSelector = ".accueil a";
            NameSelector = "#contenu h2";
            ChapterNameSelector = "h3";

            PatternPageNumber = "\\d+.html";
            PatternChapterNumber = "page \\d+";
            WrongParts = new string[] { };
        }

        public override async Task<List<Link>> GetAllLinks(string url, int fromChapterNumber)
        {
            var links = new List<Link>();
            var angleScrapService = new AngleScrapService();
            IBrowsingContext context = angleScrapService.GetContext();
            var elements = await angleScrapService.GetElements(context, url, LinkSelector);

            foreach (var element in elements.Reverse().Skip(fromChapterNumber))
            {
                var elem = element as IHtmlAnchorElement;
                var page = await angleScrapService.GetElement(context, elem.Href, PageSelector);
                var pages = page.QuerySelectorAll(ListPageSelector);
                var chapterName = page.QuerySelector(ChapterNameSelector);
                pages.ToList().ForEach(p => links.Add(new Link() { Href = ReplacePageNumber(elem.Href, p.TextContent), Name = ReplaceChapterNumber(ReplaceUnauthorizedCharacter(chapterName.TextContent, "[?|:|\"|\\n|/|/]"), p.TextContent) }));
            }

            return links;
        }
    }
}
