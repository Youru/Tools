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
        private static int _fromChapterNumber;

        public Process(ILogger<Process> logger, FactorySite factorySite, IDocument documentService)
        {
            _logger = logger;
            _factorySite = factorySite;
            _documentService = documentService;
        }

        public async Task<int> Run(string[] args)
        {
            SiteSelector siteSelector = GetSiteSelectorFromArgs(args);
            _siteService = _factorySite.GetSite(siteSelector.Type);
            _siteService.SetSiteSelector(siteSelector);

            var links = await _siteService.GetAllLinks(_fromChapterNumber);
            var folderName = await _siteService.GetMangaName();
            _documentService.CreateNewFolder(folderName);
            var linksToDownload = _siteService.RemoveLinksAlreadyDownload(links, folderName);

            GenerateBook(linksToDownload, folderName);

            return 1;

        }

        private SiteSelector GetSiteSelectorFromArgs(string[] args)
        {
            SiteSelector site = null;

            Parser.Default.ParseArguments<Options>(args)
            .WithParsed((options) =>
            {
                site = SiteHelper.GetSiteByUrl(options.Url);
                site.ChapterName = options.ChapterName;
                site.linkMode = options.RecoveryLinkMode;
                site.AbbreviationTitle = options.AbbreviationTitle;
                _fromChapterNumber = options.FromChapterNumber ?? 0;
            })
            .WithNotParsed((errs) =>
            {
                throw new Exception($"Error when parsing command {string.Join(",", errs)}");
            });

            return site;
        }

        private void GenerateBook(IEnumerable<Link> links, string folderName)
        {
            _logger.LogInformation($"number link to dl :{links.Count()}");
            var result = _siteService.GenerateFilesFromElements(links, folderName);

            if (!result.HasSuceed)
            {
                result = _siteService.RetryDownloadLinks(folderName, result.LinkExceptions.Select(le => le.Link));
                if (!result.HasSuceed)
                {
                    _logger.LogInformation($"Links remaining {result.LinkExceptions.Count}");
                    result.LinkExceptions.ForEach(le => _logger.LogInformation(le.Link.Href, le.Exception.Message));
                }
            }
        }
    }
}
