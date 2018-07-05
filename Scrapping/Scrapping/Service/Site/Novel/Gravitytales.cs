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
    public class Gravitytales : BaseNovel
    {
        private IReplace _replace;
        private IAngleScrap _angleScrapService;
        private IDocument _documentService;

        public Gravitytales(IReplace replace, IAngleScrap angleScrapService, IDocument documentService):base(replace,  angleScrapService,  documentService)
        {
            _replace = replace;
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
