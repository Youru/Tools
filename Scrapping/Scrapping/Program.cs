using CommandLine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scrapping
{
    public class Program
    {
        private static string url;
        private static int fromChapterNumber;
        private static AbstractUtilityService generationService;

        public static void Main(string[] args)
        {
            Execute(args).Wait();
        }

        public static async Task<int> Execute(string[] args)
        {
            var options = new ParseCommandeService();

            if (Parser.Default.ParseArguments(args, options))
            {
                fromChapterNumber = options.FromChapterNumber ?? 0;
                url = options.Url;
            }

            if (HasError())
                return 0;

            var links = await generationService.GetAllLinks(url, fromChapterNumber);
            var folderName = await generationService.GetMangaName(url);

            GenerateBook(links, folderName);

            return 1;
        }

        private static bool HasError()
        {
            bool hasError = false;
            if (String.IsNullOrEmpty(url))
            {
                Console.WriteLine("Url must be filled.");
                hasError = true;
            }
            else if (!UtilityServiceResolver.TryResolveUrl(url,ref generationService))
            {
                Console.WriteLine("This site is not supported.");
                hasError = true;
            }

            return hasError;
        }

        private static void GenerateBook(List<Model.Link> links, string folderName)
        {
            var document = new DocumentService();
            document.CreateNewFolder(folderName);

            Parallel.ForEach(links, currentLink =>
            {
                generationService.GenerateFileFromElements(currentLink, folderName);
            });
        }
    }
}
