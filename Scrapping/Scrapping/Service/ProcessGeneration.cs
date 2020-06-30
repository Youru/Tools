using CommandLine;
using Scrapping.Ioc;
using Scrapping.Model;
using Scrapping.Service;
using System;
using System.Collections.Generic;
using System.IO;
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
            _documentService = documentService;
        }

        public async Task<int> Process(string[] args)
        {
            var options = new ParseCommande();
            int fromChapterNumber = 0;
            Site site = new Site();
            ImageConverter imageConverter = new ImageConverter();
            var shouldConvertToComic = false;


            if (Parser.Default.ParseArguments(args, options))
            {
                fromChapterNumber = (options.FromChapterNumber.HasValue && options.FromChapterNumber.Value > 1) ? options.FromChapterNumber.Value - 1 : 0;
                site = _documentService.GetSites().FirstOrDefault(s => options.Url.Contains(s.Resolve));
                site.ChapterName = options.ChapterName;
                site.linkMode = options.RecoveryLinkMode;
                site.AbbreviationTitle = options.AbbreviationTitle;
                site.BaseUrl = new Uri(options.Url);
                shouldConvertToComic = options.ConvertToComic;
            }

            if (site.HasError())
                return 0;

            _siteService = Bootstrapper.ContainerTool.GetInstance<ISite>(site.Type);
            _siteService.SetSite(site);

            var links = await _siteService.GetAllLinks(options.Url, fromChapterNumber);
            var folderName = await _siteService.GetMangaName(options.Url);

            GenerateBook(links, folderName);

            if (shouldConvertToComic)
            {
                imageConverter.FormatDirectories(Directory.GetCurrentDirectory(), folderName);
                imageConverter.ToCbz($"{Directory.GetCurrentDirectory()}\\{folderName}", $"{Directory.GetCurrentDirectory()}\\Manga\\{folderName}.cbz");
            }

            return 1;
        }

        private static void GenerateBook(List<Link> links, string folderName)
        {
            _documentService.CreateNewFolder(folderName);
            var linksToDownload = _siteService.RemoveLinksAlreadyDownload(links, folderName);
            Console.WriteLine($"number link to dl :{linksToDownload.Count()}");
            Parallel.ForEach(linksToDownload, currentLink =>
            {
                _siteService.GenerateFileFromElements(currentLink, folderName);
            });

            if (_siteService.RemainingLinks.Any())
            {
                RetryDownloadLink(_siteService, folderName);
                if (_siteService.RemainingLinks.Any())
                {
                    Console.WriteLine($"Links remaining {_siteService.RemainingLinks.Count}");
                    foreach (var link in _siteService.RemainingLinks)
                    {
                        Console.WriteLine(link.Href);
                    }
                    Console.ReadLine();
                }
            }
        }

        private static void RetryDownloadLink(ISite _siteService, string folderName)
        {
            for (int i = 0; i < 5; i++)
            {
                var remainingList = new List<Link>(_siteService.RemainingLinks);
                _siteService.RemainingLinks = new List<Link>();
                Parallel.ForEach(remainingList, currentLink =>
                {
                    _siteService.GenerateFileFromElements(currentLink, folderName);
                });
            }
        }
    }
}
