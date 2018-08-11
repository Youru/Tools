using Xunit;
using NFluent;
using Scrapping;
using AngleSharp;
using System.Text;
using System.Linq;
using Scrapping.Model;
using System;
using Scrapping.Ioc;

namespace TestScrapping
{

    public class ProcessIntegrationTest
    {

        [Fact]
        public void Should_Process_Url()
        {
            var scrapping = new AngleScrap();

            IBrowsingContext context = scrapping.GetContext();

            Check.That(context).IsNotNull();

        }

        [Theory]
        [InlineData("https://www.wuxiaworld.com/novel/tales-of-demons-and-gods/tdg-chapter-330", ".fr-view")]
        [InlineData("http://www.translationnations.com/translations/stellar-transformations/st-book-16-chapter-33/", ".entry-content p u,.entry-content u ul,.entry-content p:not(:first-child):not(:last-child)")]
        //[InlineData("http://royalroadweed.blogspot.fr/2014/11/volume-1-chapter-1.html", ".cover span")]
        public async void Should_Get_Content(string link, string selector)
        {
            var scrapping = new AngleScrap();
            StringBuilder texte = new StringBuilder();
            IBrowsingContext context = scrapping.GetContext();

            var elements = await scrapping.GetElements(context, link, selector);
            elements.ToList().ForEach(e => texte.Append(e.InnerHtml));

            Check.That(texte.Length).IsGreaterThan(0);
        }

        [Fact]
        public void Should_Get_Site()
        {
            var documentService = new Document();
            var sites = documentService.GetSites().ToList();

            Check.That(sites.Count).IsGreaterThan(0);
        }

        [Theory]
        [InlineData("https://www.wuxiaworld.com/novel/tales-of-demons-and-gods/", 350)]
        [InlineData("http://www.translationnations.com/translations/stellar-transformations/", 380)]
        [InlineData("http://royalroadweed.blogspot.fr/2017/01/moonlight-sculptor-table-of-content.html", 150)]
        [InlineData("http://www.mangareader.net/niflheim", 40)]
        public async void Should_Get_Links(string url, int fromChapterNumber)
        {
            ISite generationService;
            var documentService = new Document();
            var site = documentService.GetSites().FirstOrDefault(s => url.Contains(s.Resolve));
            site.BaseUrl = new Uri(url);
            generationService = Bootstrapper.ContainerTool.GetInstance<ISite>(site.Type);
            generationService.SetSite(site);

            var links = await generationService.GetAllLinks(url, fromChapterNumber);

            Check.That(links.Count).IsGreaterThan(0);
        }

        [Theory]
        [InlineData("https://www.wuxiaworld.com/novel/tales-of-demons-and-gods/")]
        [InlineData("http://www.translationnations.com/translations/stellar-transformations/")]
        [InlineData("http://royalroadweed.blogspot.fr/2017/01/moonlight-sculptor-table-of-content.html")]
        [InlineData("http://www.mangareader.net/niflheim")]
        public async void Should_Get_Name(string url)
        {
            ISite generationService;
            var documentService = new Document();
            var site = documentService.GetSites().FirstOrDefault(s => url.Contains(s.Resolve));
            generationService = Bootstrapper.ContainerTool.GetInstance<ISite>(site.Type);
            generationService.SetSite(site);

            var name = await generationService.GetMangaName(url);

            Check.That(name).IsNotEmpty();
        }

        [Theory]
        [InlineData("https://www.wuxiaworld.com/novel/tales-of-demons-and-gods/tdg-chapter-1", "chapter 1")]
        [InlineData("http://www.translationnations.com/translations/stellar-transformations/st-book-11-chapter-49/", "chapter 49")]
        [InlineData("http://royalroadweed.blogspot.fr/2014/11/volume-1-chapter-1.html", "chapter 1")]
        [InlineData("http://www.mangareader.net/niflheim/1", "chapter 1")]
        public void Should_Generate_Link(string url, string name)
        {
            ISite generationService;
            var link = new Link() { Href = url, Name = name };
            var folderName = "toto";
            var documentService = new Document();
            var site = documentService.GetSites().FirstOrDefault(s => url.Contains(s.Resolve));
            generationService = Bootstrapper.ContainerTool.GetInstance<ISite>(site.Type);
            generationService.SetSite(site);

            generationService.GenerateFileFromElements(link, folderName);
        }

        [Theory]
        [InlineData("https://www.wuxiaworld.com/novel/terror-infinity/ti-vol-14-chapter-5-02/", 350)]
        public async void Should_Get_Links_From_Chapter(string url, int fromChapterNumber)
        {
            ISite generationService;
            var documentService = new Document();
            var site = documentService.GetSites().FirstOrDefault(s => url.Contains(s.Resolve));
            site.linkMode = LinkModeEnum.chapter;
            generationService = Bootstrapper.ContainerTool.GetInstance<ISite>(site.Type);
            generationService.SetSite(site);

            var links = await generationService.GetAllLinks(url, fromChapterNumber);

            Check.That(links.Count).IsGreaterThan(0);
        }
    }
}
