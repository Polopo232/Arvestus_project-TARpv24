using System;

namespace Arvestus_project_TARpv24.Services
{
    public class ThemeService
    {
        private readonly SettingsService _settings;

        public ThemeService(SettingsService settings)
        {
            _settings = settings;

            var theme = _settings.SelectedTheme;
            if (theme != AppTheme.Unspecified)
            {
                Application.Current!.UserAppTheme = theme;
            }
        }

        public void SetTheme(AppTheme theme)
        {
            _settings.SelectedTheme = theme;
            Application.Current!.UserAppTheme = theme;
        }

        public AppTheme GetCurrentTheme()
        {
            return _settings.SelectedTheme;
        }

        public void ToggleTheme()
        {
            var current = GetCurrentTheme();
            var newTheme = current == AppTheme.Dark ? AppTheme.Light : AppTheme.Dark;
            SetTheme(newTheme);
        }

        public void FollowSystem()
        {
            _settings.SelectedTheme = AppTheme.Unspecified;
            Application.Current!.UserAppTheme = AppTheme.Unspecified;
        }
    }
}
