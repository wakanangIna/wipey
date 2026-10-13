using System.Text.RegularExpressions;
namespace Wipey;

/// Bottom sheet for the profile: edit name/email, sign out, delete account.
public partial class ProfilePage : ContentPage
{
    public TaskCompletionSource Done { get; } = new();

    public ProfilePage()
    {
        InitializeComponent();
        var u = Api.Me; NameE.Text = u?.Name ?? ""; EmailE.Text = u?.Email ?? "";
    }

    protected override void OnDisappearing() { base.OnDisappearing(); Done.TrySetResult(); }

    async Task Close() => await Navigation.PopModalAsync(false);
    async void OnBackdrop(object? s, TappedEventArgs e) => await Close();

    void Fail(string msg) { Err.Text = msg; Err.IsVisible = true; }

    async void OnSave(object? s, EventArgs e)
    {
        var u = Api.Me; if (u is null) return;
        var name = (NameE.Text ?? "").Trim(); var email = (EmailE.Text ?? "").Trim().ToLower();
        if (name == "") { Fail("Enter your name."); return; }
        if (!Regex.IsMatch(email, @"^\S+@\S+\.\S+$")) { Fail("Enter a valid email address."); return; }
        try { await Api.UpdateProfile(u.Email, name, email); await Close(); }
        catch (Exception x) { Fail(x.Message); }
    }

    async void OnSignOut(object? s, EventArgs e) { await Close(); Api.SignOut(); App.GoLogin(); }

    async void OnDelete(object? s, EventArgs e)
    {
        var u = Api.Me; if (u is null) return;
        // Needs a typed confirmation, so it can't be tapped by accident.
        var t = await DisplayPromptAsync("Delete account?", "This removes your photos, cleaning history, schedules and account from this device. Type DELETE to confirm.");
        if (t?.Trim() == "DELETE") { await Api.DeleteAccount(u.Email); await Close(); App.GoLogin(); }
    }
}
