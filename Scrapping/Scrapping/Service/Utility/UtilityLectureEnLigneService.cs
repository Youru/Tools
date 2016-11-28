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
    public class UtilityLectureEnLigneService : AbstractUtilityService
    {
        private string PageSelector;
        public UtilityLectureEnLigneService()
        {
            ContentSelector = "#image";
            PageSelector = "#reader";
            LinkSelector = ".accueil a";
            NameSelector = "#contenu h2";
            WrongParts = new string[] { };
        }

        public override async Task<List<Link>> GetAllLinks(string url, int fromChapterNumber)
        {
            var pagesSelector = "#head:first-child .pages option";
            var links = new List<Link>();
            var angleScrapService = new AngleScrapService();
            IBrowsingContext context = angleScrapService.GetContext();
            var elements = await angleScrapService.GetElements(context, url, LinkSelector);

            foreach (var element in elements.Reverse().Skip(fromChapterNumber))
            {
                var elem = element as IHtmlAnchorElement;
                var page = await angleScrapService.GetElement(context, elem.Href, PageSelector);
                var pages = page.QuerySelectorAll(pagesSelector);
                var chapterName = page.QuerySelector("h3");
                pages.ToList().ForEach(p => links.Add(new Link() { Href = ReplacePageNumber(elem.Href, p.TextContent), Name = ReplaceChapterNumber(ReplaceUnauthorizedCharacter(chapterName.TextContent, "[?|:|\"|\\n|/|/]"), p.TextContent) }));
            }

            return links;
        }

        protected override async Task InnerGenerateFileFromElements(Link link, string folderName)
        {
            var document = new DocumentService();
            var angleScrapService = new AngleScrapService();

            IBrowsingContext context = angleScrapService.GetContext();
            StringBuilder text = new StringBuilder();

            try
            {
                var element = (IHtmlImageElement)await angleScrapService.GetElement(context, link.Href, ContentSelector);

                if (!String.IsNullOrEmpty(element.Source))
                {
                    document.DownloadNewPicture(folderName, link.Name, element.Source);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }
        }

        private string ReplacePageNumber(string textContent, string replacement)
        {
            string pattern = "\\d+.html";
            Regex rgx = new Regex(pattern);
            return rgx.Replace(textContent, $"{replacement}.html");
        }

        private string ReplaceChapterNumber(string textContent, string replacement)
        {
            string pattern = "page \\d+";
            Regex rgx = new Regex(pattern);
            return rgx.Replace(textContent, $"page {replacement}");
        }
        
        private string ReplaceUnauthorizedCharacter(string textContent, string pattern)
        {
            string replacement = "";
            Regex rgx = new Regex(pattern);
            return rgx.Replace(textContent, replacement).Trim();
        }
    }
}
