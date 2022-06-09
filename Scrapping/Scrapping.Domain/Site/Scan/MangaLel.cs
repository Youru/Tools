using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using AngleSharp.Html.Dom;
using Scrapping.Domain.Model;
using Scrapping.Interfaces;

namespace Scrapping.DomainServices.Site.Scan
{
    public class MangaLel : BaseScan
    {
        public override SiteEnum SiteType => SiteEnum.Mangalel;
        private IReplace _replace;
        private IAngleScrap _angleScrapService;
        private IDocument _documentService;

        public MangaLel(IReplace replace, IAngleScrap angleScrapService, IDocument documentService) : base(replace, angleScrapService, documentService)
        {
            _replace = replace;
            _angleScrapService = angleScrapService;
            _documentService = documentService;
        }

        public override async Task<List<Link>> GetAllLinks(int fromChapterNumber = 0)
        {
            var elements = await _angleScrapService.GetElements(SiteSelector.Url, SiteSelector.LinkSelector);

            return elements.Select(e => new Link()
            {
                Href = e.GetAttribute("href"),
                Name = _replace.Content(((IHtmlAnchorElement)e).PathName, "", "[?|:|\"|\\n|/|/]")
            }).Skip(fromChapterNumber).ToList();
        }

        protected override async Task<Result> InnerGenerateFileFromElements(Link link, string folderName)
        {
            var Result = new Result();
            var chapterFolder = $"{folderName}\\{link.Name}";

            try
            {
                _documentService.CreateNewFolder(chapterFolder);
                var elements = await _angleScrapService.GetElements(link.Href, SiteSelector.ContentSelector);

                for (int i = 0; i <= elements.Length; i++)
                {
                    _documentService.DownloadNewPicture(chapterFolder, $"{i + 1}", ((IHtmlImageElement)elements[i]).Dataset["src"]);
                }

            }
            catch (Exception ex)
            {
                Result.HasFailed(link, ex);
            }

            return Result;
        }
    }
}
