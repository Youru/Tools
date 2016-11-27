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
    public class AbstractUtilityService : IUtilityService
    {
        protected string ContentSelector;
        protected string LinkSelector;
        protected string NameSelector;
        protected string[] WrongParts;

        public void GenerateFileFromElements(Link link, string folderName)
        {
            InnerGenerateFileFromElements(link, folderName).Wait();
        }

        public virtual async Task<List<Link>> GetAllLinks(string url, int fromChapterNumber)
        {

            var angleScrapService = new AngleScrapService();
            IBrowsingContext context = angleScrapService.GetContext();
            var elements = await angleScrapService.GetElements(context, url, LinkSelector);

            return elements.Select(e => new Link()
            {
                Href = e.GetAttribute("href"),
                Name = ReplaceUnauthorizedCharacter(((IHtmlAnchorElement)e).PathName, "[?|:|\"|\\n|/|/]")
            }).Skip(0).ToList();
        }

        public async Task<string> GetMangaName(string url)
        {
            var angleScrapService = new AngleScrapService();
            IBrowsingContext context = angleScrapService.GetContext();
            var element = await angleScrapService.GetElement(context, url, NameSelector);

            return ReplaceUnauthorizedCharacter(element.TextContent, "[?|:|\"|\\n|/|/]");
        }

        private string ReplaceUnauthorizedCharacter(string textContent, string pattern)
        {
            string replacement = "";
            Regex rgx = new Regex(pattern);
            return rgx.Replace(textContent, replacement);
        }

        private async Task InnerGenerateFileFromElements(Link link, string folderName)
        {
            var document = new DocumentService();
            var angleScrapService = new AngleScrapService();

            IBrowsingContext context = angleScrapService.GetContext();
            StringBuilder text = new StringBuilder();

            try
            {
                var elements = await angleScrapService.GetElements(context, link.Href, ContentSelector);
                elements.Where(e => !IfElementContainsWrongPart(e)).ToList().ForEach(e => text.Append(e.OuterHtml));

                if (text.Length > 0)
                {
                    document.FillNewDocument(folderName, link.Name, text);
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
    }
}
