using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScrappingNewTest.Interfaces;
using ScrappingNewTest.Model;
using AngleSharp;
using Microsoft.Extensions.Logging;

namespace ScrappingNewTest.Services.Site.Novel
{
    public class WebNovel : BaseNovel
    {
        private IReplace _replace;
        private IAngleScrap _angleScrapService;

        public WebNovel(IReplace replace, IAngleScrap angleScrapService, IDocument documentService, ILogger<WebNovel> logger) : base(replace, angleScrapService, documentService, logger)
        {
            _replace = replace;
            _angleScrapService = angleScrapService;
        }

        public override async Task<List<Link>> GetAllLinks(int fromChapterNumber = 0)
        {
            IBrowsingContext context = _angleScrapService.GetContext();
            var document = await context.OpenAsync(GetUrlApi(Site.Url));
            var webNovel = JsonConvert.DeserializeObject<Model.WebNovel>(document.Body.TextContent);

            return webNovel.Data.VolumeItems.SelectMany(v => v.ChapterItems).Where(c => c.FeeType != 1).Select(itemChapter => new Link()
            {
                Href = $"https://www.{Site.Resolve}/book/{webNovel.Data.BookInfo.BookId}/{itemChapter.Id}/{webNovel.Data.BookInfo.BookName.Replace(' ', '-')}/{itemChapter.Name.Replace(' ', '-')}",
                Name = $"{itemChapter.Index:D4} - {_replace.Content(itemChapter.Name, "", "[?|:|\"|\\n|/|/]")}"
            }).Skip(fromChapterNumber).ToList();
        }

        public override async Task<string> GetMangaName()
        {
            IBrowsingContext context = _angleScrapService.GetContext();
            var document = await context.OpenAsync(GetUrlApi(Site.Url));
            var webNovel = JsonConvert.DeserializeObject<Model.WebNovel>(document.Body.TextContent);

            return _replace.Content(webNovel.Data.BookInfo.BookName, "", "[?|:|\"|\\n|/|/]");
        }

        private string GetUrlApi(string url)
        {
            var bookId = _replace.GetBookId(url, "\\d{16}");
            return $"https://www.{Site.Resolve}/apiajax/chapter/GetChapterList?_csrfToken={Site.Token}&bookId={bookId}";
        }
    }
}
