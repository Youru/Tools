using AngleSharp;
using AngleSharp.Dom;
using Scrapping.Interfaces;
using System.Threading.Tasks;

namespace Scrapping.Services
{
    public class AngleScrap : IAngleScrap
    {

        public async Task<string> GetTextContent(string url)
        {
            var context = GetContext();

            var document = await context.OpenAsync(url);

            return document.Body.TextContent;
        }

        public async Task<IElement> GetElement(string url, string selector)
        {
            var context = GetContext();

            await context.OpenAsync(url);
            var contentSearch = context.Active.QuerySelector(selector);

            return contentSearch;
        }

        public async Task<IHtmlCollection<IElement>> GetElements(string url, string selector)
        {
            var context = GetContext();

            await context.OpenAsync(url);
            var contentSearch = context.Active.QuerySelectorAll(selector);

            return contentSearch;
        }


        private IBrowsingContext GetContext()
        {
            var configuration = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(configuration);

            return (BrowsingContext)context;
        }

    }

}
