using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Scrapping.Domain.Interfaces;
using Scrapping.Domain.Model;
using Scrapping.Interfaces;

namespace Scrapping.DomainServices.Site.Novel
{
    public class WebNovel : BaseNovel
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public Data Data { get; set; }

        public override SiteEnum SiteType => SiteEnum.Webnovel;
        private IReplace _replace;
        private IScrappingService _angleScrapService;


        public WebNovel(IReplace replace, IScrappingService angleScrapService, IDocument documentService) : base(replace, angleScrapService, documentService)
        {
            _replace = replace;
            _angleScrapService = angleScrapService;
        }

        public override async Task<List<Link>> GetAllLinks(int fromChapterNumber = 0)
        {
            var url = GetUrlApi(SiteSelector.Url);
            var scrappingBag = await _angleScrapService.GetScrappingBagWithTextBodyContent(url);
            var webNovel = JsonConvert.DeserializeObject<WebNovel>(scrappingBag.TextBodyContent);

            return webNovel.Data.VolumeItems.SelectMany(v => v.ChapterItems).Where(c => c.FeeType != 1).Select(itemChapter => new Link(
                $"https://www.{SiteSelector.Resolve}/book/{webNovel.Data.BookInfo.BookId}/{itemChapter.Id}/{webNovel.Data.BookInfo.BookName.Replace(' ', '-')}/{itemChapter.Name.Replace(' ', '-')}",
                $"{itemChapter.Index:D4} - {_replace.Content(itemChapter.Name, "", "[?|:|\"|\\n|/|/]")}"))
                .Skip(fromChapterNumber).ToList();
        }

        public override async Task<string> GetMangaName()
        {
            var url = GetUrlApi(SiteSelector.Url);
            var scrappingBag = await _angleScrapService.GetScrappingBagWithTextBodyContent(url);
            var webNovel = JsonConvert.DeserializeObject<WebNovel>(scrappingBag.TextBodyContent);

            return _replace.Content(webNovel.Data.BookInfo.BookName, "", "[?|:|\"|\\n|/|/]");
        }

        private string GetUrlApi(string url)
        {
            var bookId = _replace.GetBookId(url, "\\d{16}");
            return $"https://www.{SiteSelector.Resolve}/apiajax/chapter/GetChapterList?_csrfToken={SiteSelector.Token}&bookId={bookId}";
        }
    }
    public class Data
    {

        public BookInfo BookInfo { get; set; }
        public IEnumerable<Item> VolumeItems { get; set; }
    }

    public class BookInfo
    {
        public string BookId { get; set; }
        public string BookName { get; set; }
        public int TotalChapterNum { get; set; }
        public string NewChapterId { get; set; }
        public int NewChapterIndex { get; set; }
        public string NewChapterName { get; set; }
    }

    public class Item
    {
        public int Index { get; set; }
        public IEnumerable<ChapterItem> ChapterItems { get; set; }
    }

    public class ChapterItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Index { get; set; }
        public int FeeType { get; set; }
    }
}
