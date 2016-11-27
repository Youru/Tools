# Tools
##Scrapping : Tool to get online book from Table of content 
  
###Dependency : 
- Anglesharp
- Commandline
- Newtonjsoft
    
###Usage : Go to the folder where is the scrapping exe 
```
    -u "http://royalroadweed.blogspot.fr/2014/11/toc.html"
    -u "http://royalroadweed.blogspot.fr/2014/11/toc.html" -f 150 (To start from the chapter/link 150)
```
###Additional : 
- Add other site or selector :
```
    Create sites.json file on the same folder than scrapping exe
    
  Format : 
  [
    {
      "Resolve" : "weedsroyalroad.wordpress.com",           ==> Site hostname 
      "ContentSelector" : ".article-wrapper",               ==> Selector to get the content of the chapter
      "LinkSelector" : ".entry-content a",                  ==> Selector to get the links on TOC
      "NameSelector" : ".site-branding .site-description",  ==> Selector to get the name of the book (folder name will be create with it)
      "WrongParts" : []                                     
    }
  ]
```
