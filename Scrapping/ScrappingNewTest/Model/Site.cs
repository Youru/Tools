﻿using Microsoft.Extensions.Logging;
using System;

namespace ScrappingNewTest.Model
{
    public class Site
    {
        private readonly ILogger<Site> _logger;

        public Site(ILogger<Site> logger)
        {
            _logger = logger;
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


        public bool HasError()
        {
            bool hasError = false;
            if (String.IsNullOrEmpty(BaseUrl.ToString()))
            {
                _logger.LogError("Url must be filled.");
                hasError = true;
            }

            return hasError;
        }
    }
}
