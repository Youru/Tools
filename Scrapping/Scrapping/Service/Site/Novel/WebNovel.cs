using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using Newtonsoft.Json;
using Scrapping.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Scrapping
{
    public class WebNovel : BaseNovel
    {
        private IRegexService _regexService;
        private IAngleScrapService _angleScrapService;
        private IDocumentService _documentService;

        public WebNovel(IRegexService regexService, IAngleScrapService angleScrapService, IDocumentService documentService) : base(regexService, angleScrapService, documentService)
        {
            _regexService = regexService;
            _angleScrapService = angleScrapService;
            _documentService = documentService;
        }

        public override async Task<List<Link>> GetAllLinks(string url, int fromChapterNumber)
        {
            IBrowsingContext context = _angleScrapService.GetContext();
            var document = await context.OpenAsync(url);
            var webNovel = JsonConvert.DeserializeObject<Model.WebNovel>(document.Body.TextContent);

            return webNovel.Data.VolumeItems.SelectMany(v => v.ChapterItems).Select(itemChapter => new Link()
            {
                Href = $"https://www.{Site.Resolve}/book/{webNovel.Data.BookInfo.BookId}/{itemChapter.Id}/{webNovel.Data.BookInfo.BookName.Replace(' ', '-')}/{itemChapter.Name.Replace(' ', '-')}",
                Name = $"{itemChapter.Index:D4} - {_regexService.ReplaceContent(itemChapter.Name, "", "[?|:|\"|\\n|/|/]")}"
            }).Skip(fromChapterNumber).ToList();
        }

        public override async Task<string> GetMangaName(string url)
        {
            IBrowsingContext context = _angleScrapService.GetContext();
            var document = await context.OpenAsync(url);
            var webNovel = JsonConvert.DeserializeObject<Model.WebNovel>(document.Body.TextContent);

            return _regexService.ReplaceContent(webNovel.Data.BookInfo.BookName, "", "[?|:|\"|\\n|/|/]");
        }
    }
}
