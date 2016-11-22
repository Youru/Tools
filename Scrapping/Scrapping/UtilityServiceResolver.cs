namespace Scrapping
{
    public static class UtilityServiceResolver
    {
        public static IUtilityService ResolveUrl(string url)
        {
            if (url.Contains("wuxia"))
                return new UtilityWuxiaService();
            else if (url.Contains("translationnations.com"))
                return new UtilityTranslationService();

            return null;
        }

        public static bool TryResolveUrl(string url, ref AbstractUtilityService utilityService)
        {
            if (url.Contains("wuxia"))
                utilityService = new UtilityWuxiaService();
            else if (url.Contains("translationnations.com"))
                utilityService = new UtilityTranslationService();
            else
                utilityService = null;

            return utilityService != null;
        }
    }
}
