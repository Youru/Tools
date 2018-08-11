using CommandLine;
using Scrapping.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scrapping
{
    public class ProcessGeneration : IProcessGeneration
    {
        private static ISite _siteService;
        private static IDocument _documentService;

        public ProcessGeneration(IDocument documentService)
        {
            _documentService = Bootstrapper.ContainerTool.GetInstance<IDocument>();
        }

        public async Task<int> Process(string[] args)
        {
            var options = new ParseCommande();
            int fromChapterNumber = 0;
            Site site = new Site();

            if (Parser.Default.ParseArguments(args, options))
            {
                fromChapterNumber = (options.FromChapterNumber.HasValue && options.FromChapterNumber.Value > 1) ? options.FromChapterNumber.Value - 1 : 0;
                site = _documentService.GetSites().FirstOrDefault(s => options.Url.Contains(s.Resolve));
                site.ChapterName = options.ChapterName;
                site.linkMode = options.RecoveryLinkMode;
                site.AbbreviationTitle = options.AbbreviationTitle;
                site.BaseUrl = new Uri(options.Url);
            }

            if (site.HasError())
                return 0;

            _siteService = Bootstrapper.ContainerTool.GetInstance<ISite>(site.Type);
            _siteService.SetSite(site);

            var links = await _siteService.GetAllLinks(options.Url, fromChapterNumber);
            var folderName = await _siteService.GetMangaName(options.Url);

            GenerateBook(links, folderName);

            return 1;
        }

        private static void GenerateBook(List<Link> links, string folderName)
        {
            _documentService.CreateNewFolder(folderName);
            var linksToDownload = _siteService.RemoveLinksAlreadyDownload(links, folderName);

            Parallel.ForEach(linksToDownload, currentLink =>
            {
                _siteService.GenerateFileFromElements(currentLink, folderName);
            });
        }
    }
}
