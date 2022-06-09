using CommandLine;
using Microsoft.Extensions.Logging;
using Scrapping.Model;
using Scrapping.Helpers;
using Scrapping.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Scrapping.Domain.Interfaces;
using Scrapping.Site;
using Scrapping.Domain.Model;

namespace Scrapping
{
    public class Process
    {
        private readonly ILogger<Process> _logger;
        private readonly FactorySite _factorySite;
        private readonly IDocument _documentService;
        private static ISite _siteService;

        public Process(ILogger<Process> logger, FactorySite factorySite, IDocument documentService)
        {
            _logger = logger;
            _factorySite = factorySite;
            _documentService = documentService;
        }

        public async Task<int> Run(string[] args)
        {
            SiteSelector site = GetSiteFromArgs(args);
            _siteService = _factorySite.GetSite(site.Type);
            _siteService.SetSite(site);

            var links = await _siteService.GetAllLinks();
            var folderName = await _siteService.GetMangaName();


            GenerateBook(links, folderName);

            return 1;

        }

        private SiteSelector GetSiteFromArgs(string[] args)
        {
            SiteSelector site = null;

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

        private async void GenerateBook(List<Link> links, string folderName)
        {
            _documentService.CreateNewFolder(folderName);
            var linksToDownload = _siteService.RemoveLinksAlreadyDownload(links, folderName);
            _logger.LogInformation($"number link to dl :{linksToDownload.Count()}");
            var remainingLinks = await _siteService.GenerateFilesFromElements(linksToDownload, folderName);

            if (remainingLinks.Any())
            {
                remainingLinks = await _siteService.RetryDownloadLinks(folderName, remainingLinks);
                if (remainingLinks.Any())
                {
                    _logger.LogInformation($"Links remaining {remainingLinks.ToList().Count}");
                    foreach (var link in remainingLinks)
                    {
                        _logger.LogInformation(link.Href);
                    }
                }
            }
        }
    }
}
