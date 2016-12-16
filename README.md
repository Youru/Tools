# Tools
##Scrapping : Tool to get online book from Table of content 
  
###Dependency : 
- Anglesharp
- Commandline
- Newtonjsoft
- Structure Map
    
###Usage : Go to the folder where the scrapping exe is
```
    -u "http://royalroadweed.blogspot.fr/2014/11/toc.html"
    -u "http://royalroadweed.blogspot.fr/2014/11/toc.html" -f 150 (To start from the chapter/link 150)
```
###Additional : 
- Add other site or selector :
```
    Update sites.json file on Datasource
    
  Format : 
  [
    {
      "Resolve" : "weedsroyalroad.wordpress.com",           // Site hostname 
      "ContentSelector" : ".article-wrapper",               // Selector to get the content of the chapter
      "LinkSelector" : ".entry-content a",                  // Selector to get the links on TOC
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
