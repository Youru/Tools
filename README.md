# Tools
## Scrapping : Tool to get online book from Table of content 
  
### Dependency : 
- Anglesharp
- Commandline
- Newtonjsoft
- Structure Map
    
### Usage : Go to the folder where the scrapping exe is
```
    General case
    -u "http://www.wuxiaworld.com/wmw-index/"
    -u "http://www.wuxiaworld.com/wmw-index/" -f 150 (To start from the chapter/link 150)
    -u "http://www.wuxiaworld.com/ti-index/ti-vol-17-chapter-5-1/" -r 1 (To start from this url)
    
    
    /!\ Specific gravitytales /!\ (Content added dynamically)
    -u "http://gravitytales.com/novel/the-kings-avatar/"  -c "/tka-chapter-" -f 100 (Number of chapter to find)
    
    Will get all chapters from
    http://gravitytales.com/novel/the-kings-avatar/tka-chapter-0 
    to 
    http://gravitytales.com/novel/the-kings-avatar/tka-chapter-100
```
### Additional : 
- Add other site or selector :
```
    Update sites.json file on Datasource
    
  Format : 
  [
    {
      "Resolve" : "weedsroyalroad.wordpress.com",           // Site hostname 
      "ContentSelector" : ".article-wrapper",               // Selector to get the content of the chapter
      "LinkSelector" : ".entry-content a",                  // Selector to get the links on TOC
      "NextChapterSelector": "div[itemprop='articleBody'] > p a", // Selector to get the next chapter link
      "NextChapterText": "Next Chapter",                    // text to help find the right next chapter link
      "NameSelector" : ".site-branding .site-description",  // Selector to get the book's name (folder name will be create with it)
      "WrongParts" : [],
      "Type": "novel",                                      // Supported type novel/scan/lec 
      // Only for scan and lec
      "PageSelector": "#reader",                            // Selector to get the full page
      "ListPageSelector": "#head:first-child .pages option",// Selector to get the chapter's page number
      "ChapterNameSelector": "h3",                          // Selector to get the name o
      "PatternPageNumber": "\\d+.html",                     // Pattern to rewrite the page's number url
      "PatternChapterNumber": "page \\d+",                  // Pattern to rewrite the chapter's number url
    }
  ]
```
