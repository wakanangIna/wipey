namespace Wipey;

/// The main screen: all four tabs live here and the bottom bar never moves, so switching tabs is instant.
public partial class HostPage : ContentPage
{
    readonly Dictionary<string, (View View, Func<Task> Shown)> tabs;
    string current = "home";

    public HostPage()
    {
        InitializeComponent();
        tabs = new()
        {
            ["home"] = (HomeV, HomeV.OnShown),
            ["gallery"] = (GalleryV, GalleryV.OnShown),
            ["schedule"] = (ScheduleV, ScheduleV.OnShown),
            ["log"] = (LogV, LogV.OnShown),
        };
        Bar.TabSelected += async route => await Show(route);
        Bar.ProfileChanged += () => HomeV.SetGreeting();
        HomeV.GoTo += async route => await Show(route);
        Bar.SetActive("home");
        _ = HomeV.OnShown();
    }

    async Task Show(string route)
    {
        if (route == "camera")
        {
            if (!await CameraFlow.Capture()) return;   // cancelled or no permission: stay where you are
            route = "gallery";
        }
        foreach (var (key, t) in tabs) t.View.IsVisible = key == route;
        current = route; Bar.SetActive(route);
        await tabs[route].Shown();
    }

    // Back button: from any other tab go to Home first, then leave the app.
    protected override bool OnBackButtonPressed()
    {
        if (current != "home") { _ = Show("home"); return true; }
        return base.OnBackButtonPressed();
    }
}
