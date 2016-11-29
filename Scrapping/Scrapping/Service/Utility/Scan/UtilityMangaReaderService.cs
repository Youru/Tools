namespace Scrapping
{
    public class UtilityMangaReaderService : AbstractScanUtilityService
    {
        public UtilityMangaReaderService()
        {
            ContentSelector = "#img";
            PageSelector = "#topchapter";
            ListPageSelector = "#selectpage option";
            LinkSelector = "#chapterlist a";
            NameSelector = "#mangaproperties .aname";
            ChapterNameSelector = "#mangainfo div:first-child";

            PatternPageNumber = "\\d+.html";
            PatternChapterNumber = "Page \\d+";
            WrongParts = new string[] { };
        }
    }
}
