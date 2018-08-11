using Xunit;
using NFluent;
using Scrapping;
using Scrapping.Ioc;

namespace TestScrapping
{

    public class UtilityServiceTest
    {

        [Theory]
        [InlineData("http://www.wuxiaworld.com/tdg-index/", 450)]
        [InlineData("http://www.translationnations.com/translations/stellar-transformations/", 400)]
        //[InlineData("http://royalroadweed.blogspot.fr/2017/01/moonlight-sculptor-table-of-content.html", 200)]
        [InlineData("http://www.mangareader.net/niflheim", 60)]
        public async void Should_Access_Site(string url, int fromChapterNumber)
        {
            string[] args = { "-u", url, "-f", fromChapterNumber.ToString() };
            IProcessGeneration process = Bootstrapper.ContainerTool.GetInstance<IProcessGeneration>();
            int result = await process.Process(args);
            Check.That(result).IsEqualTo(1);
        }
    }
}
