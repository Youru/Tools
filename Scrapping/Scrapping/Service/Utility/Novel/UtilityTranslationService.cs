namespace Scrapping
{
    public class UtilityTranslationService : AbstractNovelUtilityService
    {
        public UtilityTranslationService()
        {
            ContentSelector = ".entry-content p u,.entry-content u ul,.entry-content p:not(:first-child):not(:last-child)";
            LinkSelector = ".entry-content a";
            NameSelector = ".entry-header > h1";
            WrongParts = new string[]{ "Translated by", "Edited by" };
        }
    }
}
