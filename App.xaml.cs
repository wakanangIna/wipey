namespace Wipey;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        UserAppTheme = Preferences.Get("theme", "") switch { "dark" => AppTheme.Dark, "light" => AppTheme.Light, _ => AppTheme.Unspecified };
    }

    protected override Window CreateWindow(IActivationState? activationState)
        => new Window(Api.Me is null ? new LoginPage() : new HostPage());

    /// Navigation of whatever page is currently the window's root (used to open the bottom sheets).
    public static INavigation Nav => Current!.Windows[0].Page!.Navigation;

    public static void GoLogin() => Current!.Windows[0].Page = new LoginPage();
    public static void GoMain() => Current!.Windows[0].Page = new HostPage();
}
