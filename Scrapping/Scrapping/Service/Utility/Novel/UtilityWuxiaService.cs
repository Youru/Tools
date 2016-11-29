namespace Scrapping
{
    public class UtilityWuxiaService : AbstractNovelUtilityService
    {
        public UtilityWuxiaService()
        {
            ContentSelector = "div[itemprop='articleBody'] p:not(:first-child):not(:last-child)";
            LinkSelector = "div[itemprop='articleBody'] a";
            NameSelector = "header > h1";
            WrongParts = new string[] { "Previous Chapter", "Teaser for Next Chapter" };
        }
    }
}
