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
}
