using Xunit;
using NFluent;
using Scrapping;
using AngleSharp;
using AngleSharp.Dom;
using System.Text;
using System.Linq;

namespace TestScrapping
{

    public class ScrappingTest
    {

        [Fact]
        public void ShouldAccessSite()
        {
            var scrapping = new AngleScrapService();

            IBrowsingContext context = scrapping.GetContext();

            Check.That(context).IsNotNull();

        }

        [Theory]
        [InlineData("http://www.wuxiaworld.com/tdg-index/tdg-chapter-330/", "div[itemprop='articleBody'] p:not(:first-child):not(:last-child)")]
        [InlineData("http://www.translationnations.com/translations/stellar-transformations/st-book-16-chapter-33/", ".entry-content p u,.entry-content u ul,.entry-content p:not(:first-child):not(:last-child)")]
        [InlineData("http://royalroadweed.blogspot.fr/2014/11/volume-1-chapter-1.html", ".cover span")]
        public async void ShouldGetContent(string link, string selector)
        {
            var scrapping = new AngleScrapService();
            StringBuilder texte = new StringBuilder();
            IBrowsingContext context = scrapping.GetContext();

            var elements = await scrapping.GetElements(context, link, selector);
            elements.ToList().ForEach(e => texte.Append(e.InnerHtml));

            Check.That(texte.Length).IsGreaterThan(0);
        }

        [Fact]
        public void ShouldGetGenerationWuxiaService()
        {
            IUtilityService generationService;
            generationService = UtilityServiceResolver.ResolveUrl("wuxiaworld.com/tdg-index/");

            Check.That(generationService).IsInstanceOf<UtilityWuxiaService>();
        }

        [Fact]
        public void ShouldGetGenerationTranslationService()
        {
            IUtilityService generationService;
            generationService = UtilityServiceResolver.ResolveUrl("translationnations.com/translations/stellar-transformations/");

            Check.That(generationService).IsInstanceOf<UtilityTranslationService>();
        }

        [Fact]
        public void ShouldGetGenerationRoyalroadWeedService()
        {
            IUtilityService generationService;
            generationService = UtilityServiceResolver.ResolveUrl("royalroadweed.blogspot.fr/2014/11/toc.html");

            Check.That(generationService).IsInstanceOf<UtilityRoyalroadWeedService>();
        }

        [Theory]
        [InlineData("http://www.wuxiaworld.com/tdg-index/", 350)]
        [InlineData("http://www.translationnations.com/translations/stellar-transformations/", 380)]
        [InlineData("http://royalroadweed.blogspot.fr/2014/11/toc.html", 150)]
        public async void ShouldGetLinks(string url, int fromChapterNumber)
        {
            IUtilityService generationService;

            generationService = UtilityServiceResolver.ResolveUrl(url);
            var links = await generationService.GetAllLinks(url, fromChapterNumber);

            Check.That(links.Count).IsGreaterThan(0);
        }

        [Theory]
        [InlineData("http://www.wuxiaworld.com/tdg-index/")]
        [InlineData("http://www.translationnations.com/translations/stellar-transformations/")]
        [InlineData("http://royalroadweed.blogspot.fr/2014/11/toc.html")]
        public async void ShouldGetName(string url)
        {
            IUtilityService generationService;

            generationService = UtilityServiceResolver.ResolveUrl(url);
            var name = await generationService.GetMangaName(url);

            Check.That(name).IsNotEmpty();
        }
    }
}
