namespace Scrapping
{
    public class AdditionnalSiteUtilityService : AbstractNovelUtilityService
    {

        public AdditionnalSiteUtilityService(string contentSelector, string linkSelector, string nameSelector, string[] wrongParts)
        {
            ContentSelector = contentSelector;
            LinkSelector = linkSelector;
            NameSelector = nameSelector;
            WrongParts = wrongParts;
        }
    }
}
