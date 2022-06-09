using System.Threading.Tasks;
using AngleSharp.Dom;

namespace Scrapping.Interfaces
{
    public interface IAngleScrap
    {
        Task<IElement> GetElement(string url, string selector);
        Task<IHtmlCollection<IElement>> GetElements(string url, string selector);
        Task<string> GetTextContent(string url);
    }
}