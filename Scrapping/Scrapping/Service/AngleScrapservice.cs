using AngleSharp;
using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrapping
{
    public class AngleScrapService : IAngleScrapService
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
