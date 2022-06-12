using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using Scrapping.Interfaces;
using Microsoft.Extensions.Logging;

namespace Scrapping.Services
{
    public class Document : IDocument
    {
        private readonly ILogger<Document> _logger;
        public Document(ILogger<Document> logger)
        {
            _logger = logger;
        }
        public void CreateNewFolder(string folderName)
        {
            var currentPath = Directory.GetCurrentDirectory();
            var folderPath = Path.Combine(currentPath, folderName);

            Directory.CreateDirectory(folderPath);
        }

        public void FillNewDocument(string folderName, string fileName, StringBuilder texte)
        {
            string filePath = $"{Directory.GetCurrentDirectory()}\\{folderName}\\{fileName}.html";

            try
            {
                File.WriteAllText(filePath, texte.ToString(), Encoding.UTF8);
                _logger.LogInformation($"{fileName} has been downloaded");
            }
            catch (Exception ex)
            {
                _logger.LogError($"The generation of the {folderName}/{fileName} has encoutered an issue. ERROR : {ex.Message}");
                throw;
            }
        }

        public void DownloadNewPicture(string folderName, string fileName, string url, string chapter = null)
        {
            CreateNewFolder($@"{folderName}\{chapter}");

            string filePath = chapter != null ? $"{Path.Combine(Directory.GetCurrentDirectory(), folderName, chapter, fileName)}.jpg"
                : $"{Path.Combine(Directory.GetCurrentDirectory(), folderName, fileName)}.jpg";

            try
            {

                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(new Uri(url.Trim()), filePath);
                }
                _logger.LogInformation($"{fileName} has been downloaded");
            }
            catch (Exception ex)
            {
                _logger.LogError($"The generation from url {url} has encoutered an issue. ERROR : {ex.Message}");
                throw;
            }
        }

        public string[] GetAllFiles(string folderName)
        {
            var currentPath = Directory.GetCurrentDirectory();
            return Directory.GetFiles(currentPath + "\\" + folderName);
        }

        public IEnumerable<string> GetAllFolders(string folderName)
        {
            var currentPath = Directory.GetCurrentDirectory();
            return Directory.EnumerateDirectories(currentPath + "\\" + folderName);
        }
    }
}
