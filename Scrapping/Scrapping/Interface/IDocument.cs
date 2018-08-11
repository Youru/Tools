using System.Collections.Generic;
using System.Text;
using Scrapping.Model;

namespace Scrapping
{
    public interface IDocument
    {
        void CreateNewFolder(string folderName);
        void DownloadNewPicture(string folderName, string fileName, string url, string chapter = null);
        void FillNewDocument(string folderName, string fileName, StringBuilder texte);
        IEnumerable<Site> GetSites();
        string[] GetAllFiles(string folderName);
        IEnumerable<string> GetAllFolders(string folderName);
    }
}