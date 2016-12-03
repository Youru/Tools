using System.Collections.Generic;
using System.Text;
using Scrapping.Model;

namespace Scrapping
{
    public interface IDocumentService
    {
        void CreateNewFolder(string folderName);
        void DownloadNewPicture(string folderName, string fileName, string url);
        void FillNewDocument(string folderName, string fileName, StringBuilder texte);
        IEnumerable<Site> GetSites();
        IEnumerable<Site> GetAdditionnalSites();
        string[] GetAllPath(string folderName);
    }
}