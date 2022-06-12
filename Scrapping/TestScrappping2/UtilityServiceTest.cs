using Xunit;
using NFluent;
using Scrapping;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace TestScrapping
{

    public class UtilityServiceTest
    {

        [Theory]
        //[InlineData("http://www.translationnations.com/translations/stellar-transformations/", 400)]
        //[InlineData("http://royalroadweed.blogspot.fr/2017/01/moonlight-sculptor-table-of-content.html", 200)]
        [InlineData("https://mangakakalot.to/vagabond-4", 10)]
        public async void Should_Access_Site(string url, int fromChapterNumber)
        {
            //string[] args = { "-u", url, "-f", fromChapterNumber.ToString() };
            string[] args = { "-u", url};
            using IHost host = Program.CreateHostBuilder(args).Build();

            var process = host.Services.GetRequiredService<Process>();
            int result = await process.Run(args);
            Check.That(result).IsEqualTo(1);
        }
    }
}
