using System.Collections.Generic;
using System.Threading.Tasks;
using ScrappingNewTest.Model;
using AngleSharp;
using System.Linq;
using System.Text;
using System;
using ScrappingNewTest.Interfaces;
using AngleSharp.Html.Dom;
using Microsoft.Extensions.Logging;

namespace ScrappingNewTest.Services.Site.Scan
{
    public class MangaLel : BaseScan
    {
        private IReplace _replace;
        private IAngleScrap _angleScrapService;
        private IDocument _documentService;
        private ILogger<MangaLel> _logger;

        public MangaLel(IReplace replace, IAngleScrap angleScrapService, IDocument documentService, ILogger<MangaLel> logger) : base(replace, angleScrapService, documentService, logger)
        {
            _replace = replace;
            _angleScrapService = angleScrapService;
            _documentService = documentService;
            _logger = logger;
        }

        public override async Task<List<Link>> GetAllLinks(int fromChapterNumber = 0)
        {
            IBrowsingContext context = _angleScrapService.GetContext();
            var elements = await _angleScrapService.GetElements(context, Site.Url, Site.LinkSelector);

            return elements.Select(e => new Link()
            {
                Href = e.GetAttribute("href"),
                Name = _replace.Content(((IHtmlAnchorElement)e).PathName, "", "[?|:|\"|\\n|/|/]")
            }).Skip(fromChapterNumber).ToList();
        }

        protected override async Task InnerGenerateFileFromElements(Link link, string folderName)
        {
            IBrowsingContext context = _angleScrapService.GetContext();
            StringBuilder text = new StringBuilder();
            var chapterFolder = $"{folderName}\\{link.Name}";

            _logger.LogInformation($"Trying to dl {link.Name} with url {link.Href}");
            try
            {
                _documentService.CreateNewFolder(chapterFolder);
                var elements = await _angleScrapService.GetElements(context, link.Href, Site.ContentSelector);

                for (int i = 0; i <= elements.Length; i++)
                {
                    _documentService.DownloadNewPicture(chapterFolder, $"{i + 1}", ((IHtmlImageElement)elements[i]).Dataset["src"]);
                }

                _logger.LogInformation($"{link.Name} has been downloaded");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}
