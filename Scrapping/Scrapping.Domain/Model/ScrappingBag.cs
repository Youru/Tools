using System.Collections.Generic;
using System.Text;

namespace Scrapping.Domain.Model
{
    public class ScrappingBag
    {
        public string Title { get; private set; }
        public string Url { get; private set; }
        public string TextContent { get; private set; }
        public string Text { get; private set; }
        public string TextBodyContent { get; private set; }
        public string Source { get; private set; }
        public List<string> Sources { get; private set; }
        public StringBuilder ChapterContent { get; private set; }
        public List<Link> Links { get; private set; } = new List<Link>();
        public Link Link { get; private set; }
        //public (string, string) UrlAndText { get; private set; }

        public void SetTitle(string title) => Title = title;
        public void SetUrl(string url) => Url = url;
        public void SetTextContent(string textContent) => TextContent = textContent;
        public void SetTextBodyContent(string textBodyContent) => TextBodyContent = textBodyContent;
        public void SetSource(string source) => Source = source;
        public void SetSources(string source) => Sources.Add(source);
        public void SetChapterContent(StringBuilder chapterContent) => ChapterContent = chapterContent;
        public void SetUrlAndText(string url, string text)
        {
            Link = new Link(url, text);
            Url = url;
            Text = text;
        }
        public void SetLinks(string url, string name) => Links.Add(new Link(url, name));
        public void SetLinksWithChapter(string url, string name, string chapter) => Links.Add(new Link(url, name, chapter));

    }
}
