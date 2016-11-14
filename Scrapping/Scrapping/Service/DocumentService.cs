using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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
    }
}
