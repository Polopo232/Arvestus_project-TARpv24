using System;
using System.Threading.Tasks;

namespace Arvestus_project_TARpv24.Services
{
    public class SoundService
    {
        private readonly SettingsService _settings;

        public SoundService(SettingsService settings)
        {
            _settings = settings;
        }

        public async Task PlayClickSoundAsync()
        {
            if (!_settings.SoundEnabled) return;
            await PlayBeepAsync(800, 80);
        }

        public async Task PlaySuccessSoundAsync()
        {
            if (!_settings.SoundEnabled) return;
            await PlayBeepAsync(1000, 150);
            await Task.Delay(200);
            await PlayBeepAsync(1200, 150);
        }

        public async Task PlayErrorSoundAsync()
        {
            if (!_settings.SoundEnabled) return;
            await PlayBeepAsync(400, 200);
        }

        private async Task PlayBeepAsync(int frequency, int duration)
        {
            if (!_settings.SoundEnabled) return;
            try
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
#if WINDOWS
                    Console.Beep(frequency, duration);
#endif
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Sound error: {ex.Message}");
            }
        }
    }
}
