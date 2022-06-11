using System;
using System.Collections.Generic;

namespace Scrapping.Domain.Model
{
    public class SiteSelector
    {

        public SiteSelector()
        {
        }

        public Uri BaseUrl => new Uri(Url);
        public string Url;
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


        public List<string> Validate()
        {
            List<string> results = new();
            if (String.IsNullOrEmpty(BaseUrl.ToString()))
            {
                results.Add("Url Must Be Filled");
            }

            return results;
        }
    }
}
