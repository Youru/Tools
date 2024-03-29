﻿using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Scrapping.Domain.Interfaces;
using Scrapping.Domain.Model;
using Scrapping.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrapping.Services
{
    public class AngleScrap : IScrappingService, IScrappingTechnical
    {

        private IReplace _replace;


        public AngleScrap(IReplace replace)
        {
            _replace = replace;
        }

        public async Task<Dictionary<string, string>> GetDataset(string url, string selector)
        {
            var contentSearch = await GetElement(url, selector) as IHtmlDivElement;

            return contentSearch.Dataset.ToDictionary(d => d.Key, d => d.Value);
        }

        public async Task<string[]> GetUrls(string url, string selector)
        {
            var elements = await GetElements(url, selector);

            return elements.Select(e => ((IHtmlAnchorElement)e).Href).ToArray();
        }

        public async Task<Dictionary<int, string>> GetDatasetsByIndex(string url, string selector)
        {
            var contentSearch = await GetElements(url, selector);

            return contentSearch.SelectMany(c => ((IHtmlElement)c).Dataset).Select((d, index) => new { val = d.Value, idx = index }).ToDictionary(d => d.idx, d => d.val);
        }

        public async Task<ScrappingBag> GetScrappingBagWithTextContent(string url, string selector)
        {
            ScrappingBag scrappingBag = new();
            var contentSearch = await GetElement(url, selector);

            scrappingBag.SetTextContent(contentSearch.TextContent);

            return scrappingBag;
        }
        public async Task<ScrappingBag> GetScrappingBagWithSource(string url, string selector)
        {
            ScrappingBag scrappingBag = new();
            var contentSearch = await GetElement(url, selector) as IHtmlImageElement;
            if (contentSearch is null) return scrappingBag;

            scrappingBag.SetSource(contentSearch.Source);

            return scrappingBag;
        }
        public async Task<ScrappingBag> GetScrappingBagWithChapterContent(string url, string selector, string[] wrongParts)
        {
            ScrappingBag scrappingBag = new();
            StringBuilder chapterContent = new StringBuilder();

            var elements = await GetElements(url, selector);
            elements.Where(e => !IfElementContainsWrongPart(e, wrongParts)).ToList().ForEach(e => chapterContent.Append(e.OuterHtml));
            scrappingBag.SetChapterContent(chapterContent);

            return scrappingBag;
        }
        public async Task<ScrappingBag> GetScrappingBagWithLink(string url, string selector)
        {
            ScrappingBag scrappingBag = new();

            var hrefList = await GetElements(url, selector);
            var element = hrefList.FirstOrDefault();

            if (element is null) return scrappingBag;

            var text = _replace.Content(((IHtmlAnchorElement)element)?.PathName, "", "[*|?|:|\"|\\n|/|/]");

            scrappingBag.SetLink(element?.GetAttribute("href"), text);


            return scrappingBag;
        }
        public async Task<List<ScrappingBag>> GetScrappingBagWithUrls(string nextChapterUrl, string selector)
        {
            List<ScrappingBag> scrappingsBags = new();
            var elements = await GetElements(nextChapterUrl, selector);

            foreach (var element in elements)
            {
                ScrappingBag scrappingBag = new();
                var text = _replace.Content(((IHtmlAnchorElement)element)?.PathName, "", "[*|?|:|\"|\\n|/|/]");

                scrappingBag.SetLink(element?.GetAttribute("href"), text);

                scrappingsBags.Add(scrappingBag);
            }

            return scrappingsBags;
        }
        public async Task<ScrappingBag> GetScrappingBagWithNextPageUrl(string url, SiteSelector siteSelector)
        {
            ScrappingBag scrappingBag = new();
            var pageSelector = await GetElements(url, siteSelector.PageSelector);

            if (pageSelector.Length > 0)
            {
                var numberPageSelection = await GetElements(url, siteSelector.ListPageSelector);
                var nextUrl = $"{siteSelector.Url}?page={Int32.Parse(numberPageSelection[0].TextContent) + 1}";
                scrappingBag.SetLink(nextUrl);
            }

            return scrappingBag;
        }
        public async Task<ScrappingBag> GetScrappingBagWithLinks(string url, string selector)
        {
            ScrappingBag scrappingBag = new();
            var elements = await GetElements(url, selector);

            foreach (var element in elements)
            {
                var text = _replace.Content(((IHtmlAnchorElement)element)?.PathName, "", "[*|?|:|\"|\\n|/|/]");
                scrappingBag.SetLinks(element?.GetAttribute("href"), text);

            }

            return scrappingBag;
        }
        public async Task<ScrappingBag> GetScrappingBagWithTitleLinks(string url, string selector)
        {
            ScrappingBag scrappingBag = new();
            var elements = await GetElements(url, selector);

            foreach (var element in elements)
            {
                var text = _replace.Content(((IHtmlAnchorElement)element)?.Title, "-", "[*|?|:|\"|\\n|/|/]");
                scrappingBag.SetLinks(element?.GetAttribute("href"), text);

            }

            return scrappingBag;
        }
        public async Task<ScrappingBag> GetScrappingBagWithLinksForScan(string url, int fromChapterNumber, SiteSelector siteSelector)
        {
            ScrappingBag scrappingBag = new();
            var elements = await GetElements(url, siteSelector.LinkSelector);


            foreach (IHtmlAnchorElement elem in elements.Skip(fromChapterNumber))
            {
                var page = await GetElement(elem.Href, siteSelector.PageSelector);
                if (page != null)
                {
                    var pages = page.QuerySelectorAll(siteSelector.ListPageSelector);
                    var chapterName = page.QuerySelector(siteSelector.ChapterNameSelector);
                    pages.ToList().ForEach(p =>
                    scrappingBag.SetLinksWithChapter(
                         $"{elem.Href}/{p.TextContent}",
                        $"{int.Parse(p.TextContent):D3}",
                        chapterName.TextContent));
                }
            }

            return scrappingBag;
        }
        private async Task<IElement> GetElement(string url, string selector)
        {
            var context = GetContext();

            await context.OpenAsync(url);
            var contentSearch = context.Active.QuerySelector(selector);

            return contentSearch;
        }
        private async Task<IHtmlCollection<IElement>> GetElements(string url, string selector)
        {
            var context = GetContext();

            var document = await context.OpenAsync(url);
            var contentSearch = document.QuerySelectorAll(selector);

            return contentSearch;
        }
        private IBrowsingContext GetContext()
        {
            var configuration = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(configuration);

            return context;
        }
        private bool IfElementContainsWrongPart(IElement element, string[] wrongParts)
        {
            foreach (var wrongPart in wrongParts)
            {
                if (element.InnerHtml.Contains(wrongPart))
                    return true;
            }

            return false;
        }
    }
}
