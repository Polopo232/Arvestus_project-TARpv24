using System.ComponentModel;
using System.Globalization;
using System.Resources;

namespace Arvestus_project_TARpv24.Services
{
    public class LocalizationService : INotifyPropertyChanged
    {
        private readonly SettingsService _settings;
        private readonly ResourceManager _resourceManager;

        public static LocalizationService? Instance { get; private set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public LocalizationService(SettingsService settings)
        {
            _settings = settings;
            _resourceManager = new ResourceManager("Arvestus_project_TARpv24.Resources.Localization.AppResources", typeof(LocalizationService).Assembly);

            SetCulture(_settings.SelectedLanguage);
            Instance = this;
        }

        // Indexer used as the binding source in XAML via the Translate markup extension.
        public string this[string key] => GetString(key);

        public string GetString(string key)
        {
            var value = _resourceManager.GetString(key, CultureInfo.CurrentUICulture);
            return value ?? key;
        }

        public void SetLanguage(string languageCode)
        {
            _settings.SelectedLanguage = languageCode;
            SetCulture(languageCode);

            // Empty/null property name tells bound consumers every property (incl. the indexer) changed,
            // refreshing all {loc:Translate} bindings live.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
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
