using System.Collections.Generic;
using System.Text;

namespace Scrapping.Domain.Model
{
    public class ScrappingBag
    {
        public string TextContent { get; private set; }
        public string Source { get; private set; }
        public StringBuilder ChapterContent { get; private set; }
        public List<Link> Links { get; private set; } = new List<Link>();
        public Link Link { get; private set; }

        public void SetTextContent(string textContent) => TextContent = textContent;
        public void SetSource(string source) => Source = source;
        public void SetChapterContent(StringBuilder chapterContent) => ChapterContent = chapterContent;
        public void SetLink(string url, string text = "") => Link = new Link(url, text);
        public void SetLinks(string url, string name) => Links.Add(new Link(url, name));
        public void SetLinksWithChapter(string url, string name, string chapter) => Links.Add(new Link(url, name, chapter));

    }
}
