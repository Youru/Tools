namespace Scrapping
{
    public class AdditionnalSiteUtilityService : AbstractUtilityService
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
