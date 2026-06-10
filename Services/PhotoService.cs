using System.Diagnostics;

namespace Arvestus_project_TARpv24.Services
{
    public class PhotoService
    {
        // Opens the system photo picker, copies the chosen image into the app's data
        // directory so it survives the temp file being cleaned up, and returns its path.
        // Returns null if the user cancels or picking fails.
        public async Task<string?> PickPhotoAsync()
        {
            try
            {
                var result = await MediaPicker.Default.PickPhotoAsync();
                if (result == null)
                    return null;

                var extension = Path.GetExtension(result.FileName);
                if (string.IsNullOrEmpty(extension))
                    extension = ".jpg";

                var targetPath = Path.Combine(FileSystem.AppDataDirectory, $"img_{Guid.NewGuid():N}{extension}");

                using var sourceStream = await result.OpenReadAsync();
                using var targetStream = File.Create(targetPath);
                await sourceStream.CopyToAsync(targetStream);

                return targetPath;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"PickPhoto error: {ex.Message}");
                return null;
            }
        }
    }
}
