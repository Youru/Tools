using System;

namespace Scrapping.Model
{
    public class Site
    {
        public Uri BaseUrl;
        public string Resolve;
        public string ContentSelector;
        public string LinkSelector;
        public string NextChapterSelector;
        public string NextChapterText;
        public string NameSelector;
        public string[] WrongParts;
        public string ListPageSelector;
        public string PageSelector;
        public string ChapterNameSelector;
        public string PatternPageNumber;
        public string PatternChapterNumber;
        public string Type;
        public string ChapterName;
        public string AbbreviationTitle;
        public string Token;
        public LinkModeEnum linkMode;
    }

    public static class SiteExtension
    {
        public static bool HasError(this Site site)
        {
            bool hasError = false;
            if (site == null)
            {
                Console.WriteLine("This site is not supported.");
                hasError = true;
            }
            if (String.IsNullOrEmpty(site.BaseUrl.ToString()))
            {
                Console.WriteLine("Url must be filled.");
                hasError = true;
            }

            return hasError;
        }
    }
}
