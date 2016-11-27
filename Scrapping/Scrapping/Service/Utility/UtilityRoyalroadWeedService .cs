namespace Scrapping
{
    public class UtilityRoyalroadWeedService : AbstractUtilityService
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
