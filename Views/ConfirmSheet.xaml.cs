namespace Wipey;

/// Themed bottom-sheet replacement for the plain Android "Are you sure?" popup.
public partial class ConfirmSheet : ContentPage
{
    public TaskCompletionSource<bool> Result { get; } = new();

    public ConfirmSheet(string title, string message, string yes, string no, bool danger = false, ImageSource? picture = null)
    {
        InitializeComponent();
        TitleL.Text = title; MsgL.Text = message; YesBtn.Text = yes; NoBtn.Text = no; NoBtn.IsVisible = no != "";
        if (picture is not null) { Pic.Source = picture; PicHost.IsVisible = true; }
        if (danger)
        {
            YesBtn.BackgroundColor = Color.FromArgb("#E5484D");
            YesBtn.Shadow = new Shadow { Brush = new SolidColorBrush(Color.FromArgb("#E5484D")), Offset = new Point(0, 6), Radius = 22, Opacity = 0.3f };
        }
    }

    /// Shows the sheet and returns true if the person tapped the main button.
    public static async Task<bool> Ask(string title, string message, string yes, string no, bool danger = false, ImageSource? picture = null)
    {
        var sheet = new ConfirmSheet(title, message, yes, no, danger, picture);
        await App.Nav.PushModalAsync(sheet, false);
        return await sheet.Result.Task;
    }

    /// A message with a single OK button.
    public static Task Notice(string title, string message) => Ask(title, message, "OK", "");

    protected override void OnDisappearing() { base.OnDisappearing(); Result.TrySetResult(false); }

    async void OnYes(object? s, EventArgs e) { Result.TrySetResult(true); await Navigation.PopModalAsync(false); }
    async void OnNo(object? s, EventArgs e) => await Navigation.PopModalAsync(false);
}
