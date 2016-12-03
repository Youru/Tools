using Scrapping.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Scrapping.Service
{
    public class RegexService : IRegexService
    {
        public string ReplaceContentWithPostText(string textContent, string replacement, string pattern, string postText = ".html")
        {
            Regex rgx = new Regex(pattern);
            return rgx.Replace(textContent, $"{replacement}{postText}");
        }

        public string ReplaceContentWithPreText(string textContent, string replacement, string pattern, string preText = "page ")
        {
            Regex rgx = new Regex(pattern);
            return rgx.Replace(textContent, $"{preText}{replacement}");
        }

        public string ReplaceContent(string textContent, string replace, string pattern)
        {
            Regex rgx = new Regex(pattern);
            return rgx.Replace(textContent, replace);
        }
    }
}
