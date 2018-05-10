using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Scrapping.Model;
using Newtonsoft.Json;
using System.Net;

namespace Scrapping
{
    public class DocumentService : IDocumentService
    {
        public void CreateNewFolder(string folderName)
        {
            var currentPath = Directory.GetCurrentDirectory();
            var folderPath = Path.Combine(currentPath, folderName);

            Directory.CreateDirectory(folderPath);
        }

        public void FillNewDocument(string folderName, string fileName, StringBuilder texte)
        {
            string filePath = String.Format("{0}\\{1}\\{2}.html", Directory.GetCurrentDirectory(), folderName, fileName);

            try
            {
                File.WriteAllText(filePath, texte.ToString(), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("The generation of the {0} has encoutered an issue. ERROR : {1}", fileName, ex.Message));
                throw;
            }
        }

        public void DownloadNewPicture(string folderName, string fileName, string url)
        {
            string filePath = $"{Path.Combine(Directory.GetCurrentDirectory(), folderName, fileName)}.jpg";

            try
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(new Uri(url), filePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("The generation of the {0} has encoutered an issue. ERROR : {1}", fileName, ex.Message));
                throw;
            }
        }

        public string[] GetAllPath(string folderName)
        {
            var currentPath = Directory.GetCurrentDirectory();
            return Directory.GetFiles(currentPath + "\\" + folderName);
        }

        public IEnumerable<Site> GetSites()
        {
            var pathFile = $"{AppDomain.CurrentDomain.BaseDirectory}\\DataSource\\sites.json";
            if (File.Exists(pathFile))
            {
                return JsonConvert.DeserializeObject<IEnumerable<Site>>(File.ReadAllText(pathFile));
            }
            else
            {
                return new List<Site>();
            }
        }
    }
}
