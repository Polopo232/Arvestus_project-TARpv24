using System.ComponentModel;
using System.Runtime.CompilerServices;
using Arvestus_project_TARpv24.Services;

namespace Arvestus_project_TARpv24.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly SettingsService _settings;
        private readonly ThemeService _themeService;
        private readonly LocalizationService _localization;

        public SettingsViewModel(SettingsService settings, ThemeService themeService, LocalizationService localization)
        {
            _settings = settings;
            _themeService = themeService;
            _localization = localization;
        }

        public bool IsFollowSystem
        {
            get => _settings.SelectedTheme == AppTheme.Unspecified;
            set
            {
                if (value)
                {
                    _themeService.FollowSystem();
                    OnPropertyChanged(nameof(IsFollowSystem));
                    OnPropertyChanged(nameof(IsLightTheme));
                    OnPropertyChanged(nameof(IsDarkTheme));
                }
            }
        }

        public bool IsLightTheme
        {
            get => _settings.SelectedTheme == AppTheme.Light;
            set
            {
                if (value)
                {
                    _themeService.SetTheme(AppTheme.Light);
                    OnPropertyChanged(nameof(IsFollowSystem));
                    OnPropertyChanged(nameof(IsLightTheme));
                    OnPropertyChanged(nameof(IsDarkTheme));
                }
            }
        }

        public bool IsDarkTheme
        {
            get => _settings.SelectedTheme == AppTheme.Dark;
            set
            {
                if (value)
                {
                    _themeService.SetTheme(AppTheme.Dark);
                    OnPropertyChanged(nameof(IsFollowSystem));
                    OnPropertyChanged(nameof(IsLightTheme));
                    OnPropertyChanged(nameof(IsDarkTheme));
                }
            }
        }

        public bool IsEstonian
        {
            get => _settings.SelectedLanguage == "et";
            set
            {
                if (value)
                {
                    _localization.SetLanguage("et");
                    OnPropertyChanged(nameof(IsEstonian));
                    OnPropertyChanged(nameof(IsEnglish));
                    OnPropertyChanged(nameof(IsRussian));
                }
            }
        }

        public bool IsEnglish
        {
            get => _settings.SelectedLanguage == "en";
            set
            {
                if (value)
                {
                    _localization.SetLanguage("en");
                    OnPropertyChanged(nameof(IsEstonian));
                    OnPropertyChanged(nameof(IsEnglish));
                    OnPropertyChanged(nameof(IsRussian));
                }
            }
        }

        public bool IsRussian
        {
            get => _settings.SelectedLanguage == "ru";
            set
            {
                if (value)
                {
                    _localization.SetLanguage("ru");
                    OnPropertyChanged(nameof(IsEstonian));
                    OnPropertyChanged(nameof(IsEnglish));
                    OnPropertyChanged(nameof(IsRussian));
                }
            }
        }

        public bool SoundEnabled
        {
            get => _settings.SoundEnabled;
            set
            {
                if (_settings.SoundEnabled != value)
                {
                    _settings.SoundEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        public double FontSize
        {
            get => _settings.FontSize;
            set
            {
                if (Math.Abs(_settings.FontSize - value) > 0.01)
                {
                    _settings.FontSize = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}