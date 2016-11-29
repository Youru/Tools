using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using Scrapping.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Scrapping
{
    public class AbstractScanUtilityService : IUtilityService
    {
        protected string ContentSelector;
        protected string LinkSelector;
        protected string NameSelector;
        protected string[] WrongParts;
        protected string PageSelector;
        protected string ListPageSelector;
        protected string ChapterNameSelector;

        protected string PatternPageNumber;
        protected string PatternChapterNumber;

        public void GenerateFileFromElements(Link link, string folderName)
        {
            InnerGenerateFileFromElements(link, folderName).Wait();
        }

        public virtual async Task<List<Link>> GetAllLinks(string url, int fromChapterNumber)
        {
            var links = new List<Link>();
            var angleScrapService = new AngleScrapService();
            IBrowsingContext context = angleScrapService.GetContext();
            var elements = await angleScrapService.GetElements(context, url, LinkSelector);

            foreach (var element in elements.Skip(fromChapterNumber))
            {
                var elem = element as IHtmlAnchorElement;
                var page = await angleScrapService.GetElement(context, elem.Href, PageSelector);
                var pages = page.QuerySelectorAll(ListPageSelector);
                var chapterName = page.QuerySelector(ChapterNameSelector);
                pages.ToList().ForEach(p => links.Add(new Link() { Href = ReplacePageNumber(elem.Href, p.TextContent), Name = ReplaceChapterNumber(ReplaceUnauthorizedCharacter(chapterName.TextContent, "[?|:|\"|\\n|/|/]"), p.TextContent) }));
            }

            return links;
        }

        public async Task<string> GetMangaName(string url)
        {
            var angleScrapService = new AngleScrapService();
            IBrowsingContext context = angleScrapService.GetContext();
            var element = await angleScrapService.GetElement(context, url, NameSelector);

            return ReplaceUnauthorizedCharacter(element.TextContent, "[?|:|\"|\\n|/|/]");
        }

        protected virtual async Task InnerGenerateFileFromElements(Link link, string folderName)
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

        private bool IfElementContainsWrongPart(IElement element)
        {
            foreach (var wrongPart in WrongParts)
            {
                if (element.InnerHtml.Contains(wrongPart))
                    return true;
            }
            return false;
        }

        protected string ReplacePageNumber(string textContent, string replacement)
        {
            Regex rgx = new Regex(PatternPageNumber);
            return rgx.Replace(textContent, $"{replacement}.html");
        }

        protected string ReplaceChapterNumber(string textContent, string replacement)
        {
            Regex rgx = new Regex(PatternChapterNumber);
            return rgx.Replace(textContent, $"page {replacement}");
        }

        protected string ReplaceUnauthorizedCharacter(string textContent, string pattern)
        {
            string replacement = "";
            Regex rgx = new Regex(pattern);
            return rgx.Replace(textContent, replacement).Trim();
        }
    }
}
