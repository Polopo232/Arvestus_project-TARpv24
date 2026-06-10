using Arvestus_project_TARpv24.Services;
using Arvestus_project_TARpv24.ViewModels;
using Arvestus_project_TARpv24.Views;
using Microsoft.Extensions.Logging;

namespace Arvestus_project_TARpv24
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "lunchbox.db3");

            // Services
            builder.Services.AddSingleton<DatabaseService>(s =>
                ActivatorUtilities.CreateInstance<DatabaseService>(s, dbPath));
            builder.Services.AddSingleton<SettingsService>();
            builder.Services.AddSingleton<ThemeService>();
            builder.Services.AddSingleton<SoundService>();
            builder.Services.AddSingleton<PhotoService>();
            builder.Services.AddSingleton<LocalizationService>();
            builder.Services.AddSingleton<SessionService>();

            // ViewModels + Pages
            builder.Services.AddSingleton<MenuViewModel>();
            builder.Services.AddSingleton<MenuPage>();
            builder.Services.AddSingleton<DailyMenuViewModel>();
            builder.Services.AddSingleton<DailyMenuPage>();
            builder.Services.AddSingleton<ProfileViewModel>();
            builder.Services.AddSingleton<ProfilePage>();
            builder.Services.AddSingleton<SettingsViewModel>();
            builder.Services.AddSingleton<SettingsPage>();
            builder.Services.AddTransient<AddDishViewModel>();
            builder.Services.AddTransient<AddDishPage>();
            builder.Services.AddTransient<DishDetailViewModel>();
            builder.Services.AddTransient<DishDetailPage>();

            return builder.Build();
        }
    }
}
