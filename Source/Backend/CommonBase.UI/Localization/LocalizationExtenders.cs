using CommonBase.Resources;

namespace CommonBase.UI.Localization
{
    public static class LocalizationExtenders
    {
        public static string Localize(this string key)
        {
            return UIApplication.Service<ILocalizationProvider>().Translate(key).ToString();
        }
    }
}