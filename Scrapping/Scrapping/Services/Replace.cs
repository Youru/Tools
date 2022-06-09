using Scrapping.Interfaces;
using System.Text.RegularExpressions;

namespace Scrapping.Services
{
    public class Replace : IReplace
    {
        public string ContentWithPostText(string textContent, string replacement, string pattern, string postText = ".html")
            => new Regex(pattern).Replace(textContent, $"{replacement}{postText}");

        public string ContentWithPreText(string textContent, string replacement, string pattern, string preText = "page ")
            => new Regex(pattern).Replace(textContent, $"{preText}{replacement}");

        public string Content(string textContent, string replace, string pattern)
             => new Regex(pattern).Replace(textContent, replace);

        public string GetBookId(string textContent, string pattern)
             => new Regex(pattern).Match(textContent).Value;
    }
}
