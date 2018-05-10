using System.Collections.Generic;
using System.Threading.Tasks;
using Scrapping.Model;
using AngleSharp;
using System.Linq;
using AngleSharp.Dom.Html;
using System.Text;
using System;
using System.Diagnostics;

namespace Scrapping
{
    public class MangaLel : BaseScan
    {
        private IRegexService _regexService;
        private IAngleScrapService _angleScrapService;
        private IDocumentService _documentService;

        public MangaLel(IRegexService regexService, IAngleScrapService angleScrapService, IDocumentService documentService) : base(regexService, angleScrapService, documentService)
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

        protected override async Task InnerGenerateFileFromElements(Link link, string folderName)
        {
            IBrowsingContext context = _angleScrapService.GetContext();
            StringBuilder text = new StringBuilder();
            var chapterFolder = $"{folderName}\\{link.Name}";

            Console.WriteLine($"Trying to dl {link.Name} with url {link.Href}");
            try
            {
                _documentService.CreateNewFolder(chapterFolder);
                var elements = await _angleScrapService.GetElements(context, link.Href, Site.ContentSelector);

                for (int i = 0; i <= elements.Length; i++)
                {
                    _documentService.DownloadNewPicture(chapterFolder, $"{i + 1}", ((IHtmlImageElement)elements[i]).Dataset["src"]);
                }

                Console.WriteLine($"{link.Name} has been downloaded");
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }
        }
    }
}
