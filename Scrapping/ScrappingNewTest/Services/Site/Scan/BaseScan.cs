using AngleSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScrappingNewTest.Interfaces;
using ScrappingNewTest.Model;
using AngleSharp.Html.Dom;
using Microsoft.Extensions.Logging;

namespace ScrappingNewTest.Services.Site.Scan
{
    public class BaseScan : AbstractSiteService
    {
        private IReplace _replace;
        private IAngleScrap _angleScrapService;
        private IDocument _documentService;
        private readonly ILogger<BaseScan> _logger;

        public BaseScan(IReplace replace, IAngleScrap angleScrapService, IDocument documentService, ILogger<BaseScan> logger)
        {
            _replace = replace;
            _angleScrapService = angleScrapService;
            _documentService = documentService;
            _logger = logger;
        }

        public override async Task<List<Link>> GetAllLinks(int fromChapterNumber = 0)
        {
            var links = new List<Link>();
            IBrowsingContext context = _angleScrapService.GetContext();
            var elements = await _angleScrapService.GetElements(context, Site.Url, Site.LinkSelector);

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

        public override async Task<string> GetMangaName()
        {
            IBrowsingContext context = _angleScrapService.GetContext();
            var element = await _angleScrapService.GetElement(context, Site.Url, Site.NameSelector);

            return _replace.Content(element.TextContent, "", "[?|:|\"|\\n|/|/]");
        }

        protected override async Task InnerGenerateFileFromElements(Link link, string folderName)
        {
            IBrowsingContext context = _angleScrapService.GetContext();
            StringBuilder text = new StringBuilder();

            _logger.LogInformation($"Trying to dl {link.Name} with url {link.Href}");
            try
            {
                var element = (IHtmlImageElement)await _angleScrapService.GetElement(context, link.Href, Site.ContentSelector);

                if (!String.IsNullOrEmpty(element.Source))
                {
                    _documentService.DownloadNewPicture(folderName, link.Name, element.Source, link.Chapter);
                }

                _logger.LogInformation($"{link.Name} has been downloaded");
            }
            catch (Exception ex)
            {
                RemainingLinks.Add(link);
                _logger.LogError(ex.Message);
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
