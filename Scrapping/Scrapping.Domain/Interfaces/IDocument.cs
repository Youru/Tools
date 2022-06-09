using System.Collections.Generic;
using System.Text;

namespace Scrapping.Interfaces
{
    public interface IDocument
    {
        void CreateNewFolder(string folderName);
        void DownloadNewPicture(string folderName, string fileName, string url, string chapter = null);
        void FillNewDocument(string folderName, string fileName, StringBuilder texte);
        string[] GetAllFiles(string folderName);
        IEnumerable<string> GetAllFolders(string folderName);
    }
}