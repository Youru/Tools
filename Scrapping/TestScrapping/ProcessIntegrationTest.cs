﻿using Xunit;
using NFluent;
using Scrapping;
using AngleSharp;
using System.Text;
using System.Linq;
using Scrapping.Model;

namespace TestScrapping
{

    public class ProcessIntegrationTest
    {

        [Fact]
        public void Should_Process_Url()
        {
            var scrapping = new AngleScrapService();

            IBrowsingContext context = scrapping.GetContext();

            Check.That(context).IsNotNull();

        }

        [Theory]
        [InlineData("http://www.wuxiaworld.com/tdg-index/tdg-chapter-330/", "div[itemprop='articleBody'] p:not(:first-child):not(:last-child)")]
        [InlineData("http://www.translationnations.com/translations/stellar-transformations/st-book-16-chapter-33/", ".entry-content p u,.entry-content u ul,.entry-content p:not(:first-child):not(:last-child)")]
        [InlineData("http://royalroadweed.blogspot.fr/2014/11/volume-1-chapter-1.html", ".cover span")]
        public async void Should_Get_Content(string link, string selector)
        {
            var scrapping = new AngleScrapService();
            StringBuilder texte = new StringBuilder();
            IBrowsingContext context = scrapping.GetContext();

            var elements = await scrapping.GetElements(context, link, selector);
            elements.ToList().ForEach(e => texte.Append(e.InnerHtml));

            Check.That(texte.Length).IsGreaterThan(0);
        }

        [Fact]
        public void Should_Get_Site()
        {
            var documentService = new DocumentService();
            var sites = documentService.GetSites().ToList();

            Check.That(sites.Count).IsGreaterThan(0);
        }

        [Theory]
        [InlineData("http://www.wuxiaworld.com/tdg-index/", 350)]
        [InlineData("http://www.translationnations.com/translations/stellar-transformations/", 380)]
        [InlineData("http://royalroadweed.blogspot.fr/2014/11/toc.html", 150)]
        [InlineData("http://www.mangareader.net/niflheim", 40)]
        [InlineData("https://lecture-en-ligne.com/manga/combatcontinentdouluodalu/", 40)]
        public async void Should_Get_Links(string url, int fromChapterNumber)
        {
            ISiteService generationService;
            var documentService = new DocumentService();
            var site = documentService.GetSites().FirstOrDefault(s => url.Contains(s.Resolve));
            generationService = Bootstrapper.ContainerTool.GetInstance<ISiteService>(site.Type);
            generationService.SetSite(site);

            var links = await generationService.GetAllLinks(url, fromChapterNumber);

            Check.That(links.Count).IsGreaterThan(0);
        }

        [Theory]
        [InlineData("http://www.wuxiaworld.com/tdg-index/")]
        [InlineData("http://www.translationnations.com/translations/stellar-transformations/")]
        [InlineData("http://royalroadweed.blogspot.fr/2014/11/toc.html")]
        [InlineData("http://www.mangareader.net/niflheim")]
        [InlineData("https://lecture-en-ligne.com/manga/combatcontinentdouluodalu/")]
        public async void Should_Get_Name(string url)
        {
            ISiteService generationService;
            var documentService = new DocumentService();
            var site = documentService.GetSites().FirstOrDefault(s => url.Contains(s.Resolve));
            generationService = Bootstrapper.ContainerTool.GetInstance<ISiteService>(site.Type);
            generationService.SetSite(site);

            var name = await generationService.GetMangaName(url);

            Check.That(name).IsNotEmpty();
        }

        [Theory]
        [InlineData("http://www.wuxiaworld.com/tdg-index/tdg-chapter-1/", "chapter 1")]
        [InlineData("http://www.translationnations.com/translations/stellar-transformations/st-book-11-chapter-49/", "chapter 49")]
        [InlineData("http://royalroadweed.blogspot.fr/2014/11/volume-1-chapter-1.html", "chapter 1")]
        [InlineData("http://www.mangareader.net/niflheim/1", "chapter 1")]
        [InlineData("https://lecture-en-ligne.com/combatcontinentdouluodalu/49/0/0/1.html", "chapter 49")]
        public void Should_Generate_Link(string url, string name)
        {
            ISiteService generationService;
            var link = new Link() { Href = url, Name = name };
            var folderName = "toto";
            var documentService = new DocumentService();
            var site = documentService.GetSites().FirstOrDefault(s => url.Contains(s.Resolve));
            generationService = Bootstrapper.ContainerTool.GetInstance<ISiteService>(site.Type);
            generationService.SetSite(site);

            generationService.GenerateFileFromElements(link, folderName);
        }
    }
}