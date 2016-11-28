using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scrapping
{
    public class Program
    {
        private static string url;
        private static int fromChapterNumber;
        private static AbstractUtilityService generationService;
        private static DocumentService documentService;

        public static void Main(string[] args)
        {
            documentService = new DocumentService();
            Execute(args).Wait();
        }

        public static async Task<int> Execute(string[] args)
        {
            var options = new ParseCommandeService();

            if (Parser.Default.ParseArguments(args, options))
            {
                fromChapterNumber = (options.FromChapterNumber.HasValue && options.FromChapterNumber.Value > 1) ? options.FromChapterNumber.Value - 1 : 0;
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
            var sites = documentService.GetAdditionnalSites().ToList();
            if (String.IsNullOrEmpty(url))
            {
                Console.WriteLine("Url must be filled.");
                hasError = true;
            }
            else if (!UtilityServiceResolver.TryResolveUrl(url, ref generationService, sites))
            {
                Console.WriteLine("This site is not supported.");
                hasError = true;
            }

            return hasError;
        }

        private static void GenerateBook(List<Model.Link> links, string folderName)
        {
            documentService.CreateNewFolder(folderName);
            var linksToDownload = RemoveLinksAlreadyDownload(links, folderName);

            Parallel.ForEach(linksToDownload, currentLink =>
            {
                generationService.GenerateFileFromElements(currentLink, folderName);
            });
        }

        private static List<Model.Link> RemoveLinksAlreadyDownload(List<Model.Link> links, string folderName)
        {
            var paths = documentService.GetAllPath(folderName);
            if (paths.Length > 0)
            {
                foreach (var path in paths)
                {
                    links.RemoveAll(l => path.Contains(l.Name));
                }
            }

            return links;
        }
    }
}
