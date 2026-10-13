namespace Wipey;

/// Opens the phone camera and saves the photo. Returns true only if a photo was taken and saved.
public static class CameraFlow
{
    public static async Task<bool> Capture()
    {
        try
        {
            if (!MediaPicker.Default.IsCaptureSupported) { await ConfirmSheet.Notice("Camera", "This device can't capture photos."); return false; }

            var st = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (st != PermissionStatus.Granted) st = await Permissions.RequestAsync<Permissions.Camera>();
            if (st != PermissionStatus.Granted)
            {
                if (await ConfirmSheet.Ask("Camera", "Camera access is turned off for WIPEY. Open Settings to allow it?", "Open Settings", "Not now"))
                    AppInfo.Current.ShowSettingsUI();
                return false;
            }

            var file = await MediaPicker.Default.CapturePhotoAsync();
            if (file is null) return false;
            var path = Path.Combine(FileSystem.AppDataDirectory, $"{Guid.NewGuid():N}.jpg");
            await using (var src = await file.OpenReadAsync()) await using (var dst = File.Create(path)) await src.CopyToAsync(dst);
            var caps = await Api.List<Capture>("caps"); caps.Insert(0, new Capture { Path = path }); await Api.Save("caps", caps);
            return true;
        }
        catch (PermissionException x) { await ConfirmSheet.Notice("Camera permission", x.Message); }
        catch (Exception x) { await ConfirmSheet.Notice("Camera", x.Message); }
        return false;
    }
}
