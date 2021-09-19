
namespace ScrappingNewTest.Interfaces
{
    public interface IReplace
    {
        string ContentWithPostText(string textContent, string replacement, string pattern, string postText = ".html");
        string ContentWithPreText(string textContent, string replacement, string pattern, string preText = "page ");
        string Content(string textContent, string replace, string pattern);
        string GetBookId(string textContent, string pattern);
    }
}
