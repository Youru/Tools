namespace Scrapping
{
    public class UtilityRoyalroadWeedService : AbstractNovelUtilityService
    {
        public UtilityRoyalroadWeedService()
        {
            ContentSelector = ".pagepost .cover,#content .post-content";
            LinkSelector = ".cover div a:first-child";
            NameSelector = ".title>h2>a";
            WrongParts = new string[] { };
        }


    }
}
