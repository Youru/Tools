using Scrapping.Domain.Model;
using Scrapping.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scrapping.DomainServices.Site.Novel
{
    public class Gravitytales : BaseNovel
    {
        public override SiteEnum SiteType => SiteEnum.Gravitytales;
        public Gravitytales(IReplace replace, IAngleScrap angleScrapService, IDocument documentService) : base(replace, angleScrapService, documentService)
        {
        }

        public override async Task<List<Link>> GetAllLinks(int fromChapterNumber = 0)
        {
            var listLink = new List<Link>();

            var task = Task.Run(() =>
            {
                for (int i = 0; i <= fromChapterNumber; i++)
                {
                    listLink.Add(new Link() { Name = $"chapter - {i:D3}", Href = $"{SiteSelector.Url}{SiteSelector.AbbreviationTitle}{SiteSelector.ChapterName}{i}" });
                }
            });

            await task;

            return listLink;
        }
    }
}
