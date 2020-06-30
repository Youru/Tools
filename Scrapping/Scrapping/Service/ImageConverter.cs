using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using IronPdf;

namespace Scrapping.Service
{
    public class ImageConverter
    {



        public void ToCbz(string cbzSourcePath, string cbzDestinationPath)
        {
            if (File.Exists(cbzDestinationPath))
            {
                File.Delete(cbzDestinationPath);
            }

            ZipFile.CreateFromDirectory(cbzSourcePath, cbzDestinationPath);
        }

        public void FormatDirectories(string rootPath, string mangaName)
        {
            foreach (var directory in Directory.EnumerateDirectories($"{rootPath}\\{mangaName}"))
            {
                string[] manga_name = { mangaName };
                var sub_path = directory.Split(manga_name, StringSplitOptions.RemoveEmptyEntries);
                var suffix = sub_path[sub_path.Length - 1].Trim();
                var chapter_number = $"{int.Parse(Regex.Replace(suffix, @"[^\d]", String.Empty)):D3}";
                if (!Directory.Exists($"{rootPath}\\{mangaName}\\{mangaName} {chapter_number}"))
                    Directory.Move(directory, $"{rootPath}\\{mangaName}\\{mangaName} {chapter_number}");

            }
        }
    }

    public class DirectoryNameComparer : IComparer<string>
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        static extern int StrCmpLogicalW(string x, string y);

        public int Compare(string x, string y)
        {
            return StrCmpLogicalW(x, y);
        }
    }
}
