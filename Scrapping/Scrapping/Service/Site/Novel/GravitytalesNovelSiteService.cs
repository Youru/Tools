using AngleSharp;
using AngleSharp.Dom.Html;
using Scrapping.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrapping
{
    public class GravitytalesNovelSiteService : GenericNovelSiteService
    {
        private IRegexService _regexService;
        private IAngleScrapService _angleScrapService;
        private IDocumentService _documentService;

        public GravitytalesNovelSiteService(IRegexService regexService, IAngleScrapService angleScrapService, IDocumentService documentService):base(regexService,  angleScrapService,  documentService)
        {
            _regexService = regexService;
            _angleScrapService = angleScrapService;
            _documentService = documentService;
        }

        public override async Task<List<Link>> GetAllLinks(string url, int fromChapterNumber)
        {
            var listLink = new List<Link>();

            for (int i = 0; i <= fromChapterNumber; i++)
            {
                listLink.Add(new Link() { Name = $"chapter - {i:D3}", Href = $"{url}{Site.AbbreviationTitle}{Site.ChapterName}{i}" });
            }

            return listLink;
        }
    }
}
