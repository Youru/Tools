using CommandLine;
using Microsoft.Extensions.Logging;
using ScrappingNewTest.Helpers;
using ScrappingNewTest.Interfaces;
using ScrappingNewTest.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScrappingNewTest
{
    public class Process
    {
        private readonly ILogger<Process> _logger;
        private readonly Func<string, ISite> _getSiteService;
        private readonly IDocument _documentService;
        private static ISite _siteService;

        public Process(ILogger<Process> logger, Func<string, ISite> getSiteService, IDocument documentService)
        {
            _logger = logger;
            _getSiteService = getSiteService;
            _documentService = documentService;
        }

        public async Task<int> Run(string[] args)
        {
            var options = new Options();
            Site site = GetSiteFromArgs(args);
            _siteService = _getSiteService(site.Type);
            _siteService.SetSite(site);

            var links = await _siteService.GetAllLinks();
            var folderName = await _siteService.GetMangaName();


            GenerateBook(links, folderName);

            return 1;

        }

        private Site GetSiteFromArgs(string[] args)
        {
            Site site = null;

            Parser.Default.ParseArguments<Options>(args)
            .WithParsed((options) =>
            {
                site = SiteHelper.GetSiteByUrl(options.Url);
                site.ChapterName = options.ChapterName;
                site.linkMode = options.RecoveryLinkMode;
                site.AbbreviationTitle = options.AbbreviationTitle;
            })
            .WithNotParsed((errs) =>
            {
                throw new Exception($"Error when parsing command {string.Join(",", errs)}");
            });

            return site;
        }

        private void GenerateBook(List<Link> links, string folderName)
        {
            _documentService.CreateNewFolder(folderName);
            var linksToDownload = _siteService.RemoveLinksAlreadyDownload(links, folderName);
            _logger.LogInformation($"number link to dl :{linksToDownload.Count()}");
            Parallel.ForEach(linksToDownload, currentLink =>
            {
                _siteService.GenerateFileFromElements(currentLink, folderName);
            });

            if (_siteService.RemainingLinks.Any())
            {
                RetryDownloadLink(_siteService, folderName);
                if (_siteService.RemainingLinks.Any())
                {
                    _logger.LogInformation($"Links remaining {_siteService.RemainingLinks.Count}");
                    foreach (var link in _siteService.RemainingLinks)
                    {
                        _logger.LogInformation(link.Href);
                    }
                }
            }
        }

        private void RetryDownloadLink(ISite _siteService, string folderName)
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
