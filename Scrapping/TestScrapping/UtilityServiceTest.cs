using Xunit;
using NFluent;
using Scrapping;
using AngleSharp;
using AngleSharp.Dom;
using System.Text;
using System.Linq;
using Scrapping.Model;

namespace TestScrapping
{

    public class UtilityServiceTest
    {

        [Theory]
        [InlineData("http://www.wuxiaworld.com/tdg-index/", 450)]
        [InlineData("http://www.translationnations.com/translations/stellar-transformations/", 400)]
        [InlineData("http://royalroadweed.blogspot.fr/2014/11/toc.html", 200)]
        [InlineData("http://www.mangareader.net/niflheim", 60)]
        [InlineData("https://lecture-en-ligne.com/manga/combatcontinentdouluodalu/", 48)]
        public async void Should_Access_Site(string url, int fromChapterNumber)
        {
            string[] args = { "-u", url, "-f", fromChapterNumber.ToString() };
            IProcessGeneration process = Bootstrapper.ContainerTool.GetInstance<IProcessGeneration>();
            int result = await process.Process(args);
            Check.That(result).IsEqualTo(1);
        }
    }
}
