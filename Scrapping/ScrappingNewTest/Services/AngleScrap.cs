using AngleSharp;
using AngleSharp.Dom;
using ScrappingNewTest.Interfaces;
using System.Threading.Tasks;

namespace ScrappingNewTest.Services
{
    public class AngleScrap : IAngleScrap
    {
        public IBrowsingContext GetContext()
        {
            var configuration = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(configuration);

            return (BrowsingContext)context;
        }

        public async Task<IElement> GetElement(IBrowsingContext context, string url, string selector)
        {

            await context.OpenAsync(url);
            var contentSearch = context.Active.QuerySelector(selector);

            return contentSearch;
        }

        public async Task<IHtmlCollection<IElement>> GetElements(IBrowsingContext context, string url,string selector)
        {

            await context.OpenAsync(url);
            var contentSearch = context.Active.QuerySelectorAll(selector);

            return contentSearch;
        }

    }
    
}
