using CommandLine;

namespace Scrapping
{
    public class ParseCommandeService
    {
        [Option('f', DefaultValue = 0, HelpText = "Get Chapter from")]
        public int? FromChapterNumber { get; set; }

        [Option('u',Required = true, HelpText = "Url of the book")]
        public string Url { get; set; }

        [Option('c', DefaultValue = null, HelpText = "Chapter Name for gravitytale")]
        public string ChapterName { get; set; }
    }
}
