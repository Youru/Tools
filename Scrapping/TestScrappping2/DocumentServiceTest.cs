using Xunit;
using Scrapping;
using System.Text;
using NFluent;
using System;
using System.Net;
using Scrapping.Services;

namespace TestScrapping
{

    public class DocumentServiceTest
    {
        [Fact]
        public void Should_Create_Folder()
        {
            var folderName = "Toto";
            var documentService = new Document(null);
            documentService.CreateNewFolder(folderName);
        }

        [Fact]
        public void Should_Fill_New_Document()
        {
            var folderName = "Toto";
            var fileName = "tata";
            var documentService = new Document(null);
            documentService.FillNewDocument(folderName, fileName, new StringBuilder());
        }

        [Fact]
        public void Should_Throw_Exception_When_Fill_New_Document_With_Wrong_Character()
        {
            var folderName = "Toto";
            var fileName = "*$/";
            var documentService = new Document(null);

            Check.ThatCode(() => documentService.FillNewDocument(folderName, fileName, new StringBuilder())).Throws<ArgumentException>();
        }

        [Fact]
        public void Should_Fill_New_Picture()
        {
            var folderName = "Toto";
            var fileName = "tata";
            var url = @"https://www.nasa.gov/sites/default/files/styles/image_card_4x3_ratio/public/thumbnails/image/leisa_christmas_false_color.png?itok=Jxf0IlS4";
            var documentService = new Document(null);
            documentService.DownloadNewPicture(folderName, fileName, url);
        }

        [Fact]
        public void Should_Throw_Exception_When_Fill_New_Picture_With_Wrong_Character()
        {
            var folderName = "titi";
            var fileName = "*$/?-";
            var url = @"https://www.nasa.gov/sites/default/files/styles/image_card_4x3_ratio/public/thumbnails/image/leisa_christmas_false_color.png?itok=Jxf0IlS4";
            var documentService = new Document(null);

            Check.ThatCode(() => documentService.DownloadNewPicture(folderName, fileName, url)).Throws<WebException>();
        }

        [Fact]
        public void Should_Get_All_Path_For_A_Folder()
        {
            var folderName = "Toto";
            var documentService = new Document(null);
            var path = documentService.GetAllFiles(folderName);

            Check.That(path).IsNotNull();
        }
    }
}
