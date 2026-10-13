using Ico = Microsoft.Maui.Controls.Shapes.Path;
namespace Wipey;

public partial class ProfileBar : ContentView
{
    readonly Dictionary<string, (Ico icon, Label label)> tabs = new();
    string active = "";
    public event Action<string>? TabSelected;   // "home" | "gallery" | "schedule" | "log" | "camera"
    public event Action? ProfileChanged;

    public ProfileBar()
    {
        InitializeComponent();
        Fab.Content = Icons.Make(Icons.Camera, Colors.White, 24);
        AddTab(0, "home", "Home", Icons.Home);
        AddTab(1, "gallery", "Gallery", Icons.Gallery);
        AddTab(3, "schedule", "Schedule", Icons.Calendar);
        AddTab(4, "log", "Log", Icons.Check);
        Refresh(); _ = Apply(IsDark, false);
    }

    static bool IsDark => Preferences.Get("theme", "") is "dark" || (Preferences.Get("theme", "") == "" && Application.Current?.RequestedTheme == AppTheme.Dark);
    static string Cap(string s) => s.Length == 0 ? s : char.ToUpper(s[0]) + s.Substring(1);

    void AddTab(int col, string route, string text, string data)
    {
        var icon = Icons.Make(data, Color.FromArgb("#8A8D98"), 22);
        var label = new Label { Text = text, FontSize = 11, HorizontalTextAlignment = TextAlignment.Center, TextColor = Color.FromArgb("#8A8D98") };
        var box = new VerticalStackLayout { Spacing = 3, Padding = new Thickness(0, 9, 0, 4), BackgroundColor = Colors.Transparent, Children = { icon, label } };
        box.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => TabSelected?.Invoke(route)) });
        Nav.Add(box, col, 0);
        tabs[route] = (icon, label);
    }

    void OnCamera(object? s, EventArgs e) => TabSelected?.Invoke("camera");

    /// Highlights the given tab.
    public void SetActive(string route) { active = route; MarkActive(); }

    void MarkActive()
    {
        var on = Color.FromArgb(IsDark ? "#7C8FF5" : "#5468E8"); var off = Color.FromArgb("#8A8D98");
        foreach (var (route, t) in tabs)
        {
            var c = route == active ? on : off;
            t.icon.Stroke = new SolidColorBrush(c); t.label.TextColor = c;
            t.label.FontAttributes = route == active ? FontAttributes.Bold : FontAttributes.None;
        }
    }

    public void Refresh()
    {
        var n = Api.Me is { } u ? (u.Name != "" ? u.Name : u.Email) : "Profile";
        n = Cap(n.Trim()); NameL.Text = n; Ini.Text = n.Length > 0 ? n.Substring(0, 1).ToUpper() : "?";
        MarkActive();
        _ = Apply(IsDark, false);   // each page has its own bar: re-sync the switch with the saved theme
    }

    async void OnTheme(object? s, EventArgs e)
    {
        var dark = !IsDark;
        Preferences.Set("theme", dark ? "dark" : "light");
        Application.Current!.UserAppTheme = dark ? AppTheme.Dark : AppTheme.Light;
        MarkActive();
        await Apply(dark, true);
    }

    void SetGlyph(bool dark, Color c) { Glyph.Children.Clear(); Glyph.Children.Add(Icons.Make(dark ? Icons.Moon : Icons.Sun, c, 15, 2)); }

    async Task Apply(bool dark, bool animate)
    {
        var line = Color.FromArgb(dark ? "#F6F0E6" : "#2A2E3A");
        Track.BackgroundColor = Color.FromArgb(dark ? "#0E1219" : "#FFFFFF"); Track.Stroke = line;
        Knob.BackgroundColor = Color.FromArgb(dark ? "#F6F0E6" : "#FFFFFF"); Knob.Stroke = line;
        var gc = Color.FromArgb(dark ? "#0E1219" : "#2A2E3A"); var x = dark ? 0 : 30;
        if (!animate) { SetGlyph(dark, gc); Knob.TranslationX = x; return; }
        var move = Knob.TranslateToAsync(x, 0, 450, Easing.CubicInOut);
        await Glyph.RotateToAsync(90, 200, Easing.CubicIn);
        SetGlyph(dark, gc); Glyph.Rotation = -90;
        await Glyph.RotateToAsync(0, 250, Easing.CubicOut);
        await move;
    }

    async void OnProfile(object? s, EventArgs e)
    {
        var sheet = new ProfilePage();
        await App.Nav.PushModalAsync(sheet, false);
        await sheet.Done.Task;
        if (Api.Me is null) return;   // signed out or deleted
        Refresh(); ProfileChanged?.Invoke();
    }
}
