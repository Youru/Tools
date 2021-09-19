using CommandLine;

namespace ScrappingNewTest.Model
{
    public class Options
    {
        [Option('f', Default = 0, HelpText = "Get Chapter from")]
        public int? FromChapterNumber { get; set; }

        [Option('r', Default = 0, HelpText = "Choice recovery link mode")]
        public LinkModeEnum RecoveryLinkMode { get; set; }

        //Exemple /zl-chapter-
        [Option('a', Default = "", HelpText = "Abbreviation Title for gravity Tale")]
        public string AbbreviationTitle { get; set; }

        [Option('u', Required = true, HelpText = "Url of the book")]
        public string Url { get; set; }

        [Option('c', Default = null, HelpText = "Chapter Name for gravitytale")]
        public string ChapterName { get; set; }

        [Option("convert", Required = false, HelpText = "Url of the book")]
        public bool ConvertToComic { get; set; }
    }
}
