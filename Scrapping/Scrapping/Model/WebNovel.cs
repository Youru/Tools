using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrapping.Model
{
    public class WebNovel
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public Data Data { get; set; }
    }

    public class Data
    {

        public BookInfo BookInfo { get; set; }
        public IEnumerable<Item> VolumeItems { get; set; }
    }

    public class BookInfo
    {
        public string BookId { get; set; }
        public string BookName { get; set; }
        public int TotalChapterNum { get; set; }
        public string NewChapterId { get; set; }
        public int NewChapterIndex { get; set; }
        public string NewChapterName { get; set; }
    }

    public class Item
    {
        public int Index { get; set; }
        public IEnumerable<ChapterItem> ChapterItems { get; set; }
    }

    public class ChapterItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Index { get; set; }
    }
}
