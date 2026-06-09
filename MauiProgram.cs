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

            builder.Services.AddSingleton<DatabaseService>(s =>
                ActivatorUtilities.CreateInstance<DatabaseService>(s, dbPath));

            builder.Services.AddSingleton<MenuViewModel>();
            builder.Services.AddSingleton<MenuPage>();

            return builder.Build();
        }
    }
}
