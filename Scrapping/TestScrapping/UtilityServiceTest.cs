using Xunit;
using NFluent;
using Scrapping;

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
            IProcessGenerationService process = Bootstrapper.ContainerTool.GetInstance<IProcessGenerationService>();
            int result = await process.Process(args);
            Check.That(result).IsEqualTo(1);
        }
    }
}
