using AngleSharp;
using AngleSharp.Dom.Html;
using Scrapping.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrapping
{
    public class BaseScan : AbstractSiteService
    {
        private IReplace _replace;
        private IAngleScrap _angleScrapService;
        private IDocument _documentService;

        public BaseScan(IReplace replace, IAngleScrap angleScrapService, IDocument documentService)
        {
            _replace = replace;
            _angleScrapService = angleScrapService;
            _documentService = documentService;
        }

        public override async Task<List<Link>> GetAllLinks(string url, int fromChapterNumber)
        {
            var links = new List<Link>();
            IBrowsingContext context = _angleScrapService.GetContext();
            var elements = await _angleScrapService.GetElements(context, url, Site.LinkSelector);

            foreach (var element in elements.Skip(fromChapterNumber))
            {
                var elem = element as IHtmlAnchorElement;
                var page = await _angleScrapService.GetElement(context, elem.Href, Site.PageSelector);
                if (page != null)
                {
                    var pages = page.QuerySelectorAll(Site.ListPageSelector);
                    var chapterName = page.QuerySelector(Site.ChapterNameSelector);
                    pages.ToList().ForEach(p => links.Add(new Link() { Href = $"{elem.Href}/{p.TextContent}", Chapter = chapterName.TextContent, Name = $"{int.Parse(p.TextContent):D3}" }));
                }
            }

            return links;
        }

        public override async Task<string> GetMangaName(string url)
        {
            IBrowsingContext context = _angleScrapService.GetContext();
            var element = await _angleScrapService.GetElement(context, url, Site.NameSelector);

            return _replace.Content(element.TextContent, "", "[?|:|\"|\\n|/|/]");
        }

        protected override async Task InnerGenerateFileFromElements(Link link, string folderName)
        {
            IBrowsingContext context = _angleScrapService.GetContext();
            StringBuilder text = new StringBuilder();

            Console.WriteLine($"Trying to dl {link.Name} with url {link.Href}");
            try
            {
                var element = (IHtmlImageElement)await _angleScrapService.GetElement(context, link.Href, Site.ContentSelector);

                if (!String.IsNullOrEmpty(element.Source))
                {
                    _documentService.DownloadNewPicture(folderName, link.Name, element.Source, link.Chapter);
                }

                Console.WriteLine($"{link.Name} has been downloaded");
            }
            catch (Exception ex)
            {
                RemainingLinks.Add(link);
                Trace.TraceError(ex.Message);
            }
        }

        public override List<Link> RemoveLinksAlreadyDownload(List<Link> links, string folderName)
        {
            var paths = _documentService.GetAllFolders(folderName);
            if (paths.Count() > 0)
            {
                foreach (var path in paths)
                {
                    var chapterTitle = path.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries).Last();
                    var pathFiles = Directory.GetFiles(path);

                    foreach (var pathFile in pathFiles)
                    {
                        var fileTitle = pathFile.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries).Last().Replace(".jpg", "");
                        links.RemoveAll(l => l.Name == fileTitle && l.Chapter == chapterTitle);
                    }
                }
            }

            return links;
        }
    }
}
