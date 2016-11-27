using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Scrapping.Model;
using Newtonsoft.Json;

namespace Scrapping
{
    public class DocumentService
    {
        public void CreateNewFolder(string folderName)
        {
            var currentPath = Directory.GetCurrentDirectory();

            Directory.CreateDirectory(currentPath + "\\" + folderName);
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
            }
        }

        public string[] GetAllPath(string folderName)
        {
            var currentPath = Directory.GetCurrentDirectory();
            return Directory.GetFiles(currentPath + "\\" + folderName);
        }

        public IEnumerable<Site> GetAdditionnalSites()
        {
            var pathFile = $"{Directory.GetCurrentDirectory()}\\sites.json";
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
