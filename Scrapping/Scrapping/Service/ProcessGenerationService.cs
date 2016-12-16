using CommandLine;
using Scrapping.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scrapping
{
    public class ProcessGenerationService : IProcessGenerationService
    {

        private static string url;
        private static int fromChapterNumber;
        private static Site site;

        private static ISiteService _siteService;
        private static IDocumentService _documentService;

        public ProcessGenerationService( IDocumentService documentService)
        {
            _documentService = Bootstrapper.ContainerTool.GetInstance<IDocumentService>();
        }

        public async Task<int> Process(string[] args)
        {
            var options = new ParseCommandeService();

            if (Parser.Default.ParseArguments(args, options))
            {
                fromChapterNumber = (options.FromChapterNumber.HasValue && options.FromChapterNumber.Value > 1) ? options.FromChapterNumber.Value - 1 : 0;
                url = options.Url;
                site = _documentService.GetSites().FirstOrDefault(s => url.Contains(s.Resolve));
            }

            if (HasError())
                return 0;

            _siteService = Bootstrapper.ContainerTool.GetInstance<ISiteService>(site.Type);
            _siteService.SetSite(site);
            var links = await _siteService.GetAllLinks(url, fromChapterNumber);
            var folderName = await _siteService.GetMangaName(url);

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
            else if (site == null)
            {
                Console.WriteLine("This site is not supported.");
                hasError = true;
            }

            return hasError;
        }

        private static void GenerateBook(List<Link> links, string folderName)
        {
            _documentService.CreateNewFolder(folderName);
            var linksToDownload = RemoveLinksAlreadyDownload(links, folderName);

            Parallel.ForEach(linksToDownload, currentLink =>
            {
                _siteService.GenerateFileFromElements(currentLink, folderName);
            });
        }

        private static List<Link> RemoveLinksAlreadyDownload(List<Link> links, string folderName)
        {
            var paths = _documentService.GetAllPath(folderName);
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
