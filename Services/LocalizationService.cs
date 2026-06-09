using System.Globalization;
using System.Resources;
using System.Reflection;

namespace Arvestus_project_TARpv24.Services
{
    public class LocalizationService
    {
        private readonly SettingsService _settings;
        private readonly ResourceManager _resourceManager;

        public LocalizationService(SettingsService settings)
        {
            _settings = settings;
            _resourceManager = new ResourceManager("Arvestus_project_TARpv24.Resources.Localization.AppResources", typeof(LocalizationService).Assembly);

            SetCulture(_settings.SelectedLanguage);
        }

        public string this[string key]
        {
            get
            {
                var value = _resourceManager.GetString(key);
                return value ?? key;
            }
        }

        public string GetString(string key)
        {
            var value = _resourceManager.GetString(key);
            return value ?? key;
        }

        public void SetLanguage(string languageCode)
        {
            _settings.SelectedLanguage = languageCode;
            SetCulture(languageCode);
        }

        private void SetCulture(string languageCode)
        {
            try
            {
                var culture = new CultureInfo(languageCode);
                CultureInfo.CurrentUICulture = culture;
                CultureInfo.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
                Thread.CurrentThread.CurrentCulture = culture;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Culture error: {ex.Message}");
            }
        }

        public string GetCurrentLanguage()
        {
            return _settings.SelectedLanguage;
        }
    }
}
