namespace Scrapping.Domain.Model
{
    public class Link
    {
        public string Chapter { get; set; }
        public string Name { get; init; }
        public string Href { get; init; }

        public Link(string href, string name)
        {
            Href = href;
            Name = name;
        }
        public Link(string href, string name, string chapter)
        {
            Href = href;
            Name = name;
            Chapter = chapter;
        }
    }
}
