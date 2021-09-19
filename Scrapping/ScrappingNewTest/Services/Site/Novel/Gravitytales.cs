using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ScrappingNewTest.Interfaces;
using ScrappingNewTest.Model;

namespace ScrappingNewTest.Services.Site.Novel
{
    public class Gravitytales : BaseNovel
    {

        public Gravitytales(IReplace replace, IAngleScrap angleScrapService, IDocument documentService, ILogger<Gravitytales> logger) : base(replace, angleScrapService, documentService, logger)
        {
        }

        public override async Task<List<Link>> GetAllLinks(int fromChapterNumber = 0)
        {
            var listLink = new List<Link>();

            var task = Task.Run(() =>
            {
                for (int i = 0; i <= fromChapterNumber; i++)
                {
                    listLink.Add(new Link() { Name = $"chapter - {i:D3}", Href = $"{Site.Url}{Site.AbbreviationTitle}{Site.ChapterName}{i}" });
                }
            });

            await task;

            return listLink;
        }
    }
}
