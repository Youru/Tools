﻿using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;

namespace ScrappingNewTest.Interfaces
{
    public interface IAngleScrap
    {
        IBrowsingContext GetContext();
        Task<IElement> GetElement(IBrowsingContext context, string url, string selector);
        Task<IHtmlCollection<IElement>> GetElements(IBrowsingContext context, string url, string selector);
    }
}