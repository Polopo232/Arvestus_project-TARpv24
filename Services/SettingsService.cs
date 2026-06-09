using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Arvestus_project_TARpv24.Services
{
    public class SettingsService : INotifyPropertyChanged
    {
        private const string ThemeKey = "user_theme";
        private const string LanguageKey = "user_language";
        private const string SoundEnabledKey = "sound_enabled";
        private const string FontSizeKey = "font_size";

        public event PropertyChangedEventHandler PropertyChanged;

        public string SelectedLanguage
        {
            get => Preferences.Get(LanguageKey, "et");
            set
            {
                if (Preferences.Get(LanguageKey, "et") != value)
                {
                    Preferences.Set(LanguageKey, value);
                    OnPropertyChanged();
                }
            }
        }

        public bool SoundEnabled
        {
            get => Preferences.Get(SoundEnabledKey, true);
            set
            {
                if (Preferences.Get(SoundEnabledKey, true) != value)
                {
                    Preferences.Set(SoundEnabledKey, value);
                    OnPropertyChanged();
                }
            }
        }

        public double FontSize
        {
            get => Preferences.Get(FontSizeKey, 14.0);
            set
            {
                if (Math.Abs(Preferences.Get(FontSizeKey, 14.0) - value) > 0.01)
                {
                    Preferences.Set(FontSizeKey, value);
                    OnPropertyChanged();
                }
            }
        }

        public AppTheme SelectedTheme
        {
            get => (AppTheme)Preferences.Get(ThemeKey, (int)AppTheme.Unspecified);
            set
            {
                if (Preferences.Get(ThemeKey, (int)AppTheme.Unspecified) != (int)value)
                {
                    Preferences.Set(ThemeKey, (int)value);
                    OnPropertyChanged();
                }
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
