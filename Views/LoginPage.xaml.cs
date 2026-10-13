using System.Text.RegularExpressions;
namespace Wipey;

public partial class LoginPage : ContentPage
{
    bool signUp;
    public LoginPage() => InitializeComponent();

    void OnSwap(object? s, EventArgs e)
    {
        signUp = !signUp; Err.Text = "";
        TitleLabel.Text = signUp ? "Create your account" : "Welcome back";
        Sub.Text = signUp ? "Set up WIPEY in a minute." : "Sign in to control your whiteboard robot.";
        NameE.IsVisible = signUp; Forgot.IsVisible = !signUp;
        Go.Text = signUp ? "Sign up" : "Sign in";
        Swap.Text = signUp ? "Have an account? Sign in" : "New here? Create an account";
    }
    void OnEye(object? s, EventArgs e) { PwE.IsPassword = !PwE.IsPassword; Eye.Text = PwE.IsPassword ? "Show" : "Hide"; }

    async void OnGo(object? s, EventArgs e)
    {
        var email = (EmailE.Text ?? "").Trim().ToLower(); var pw = PwE.Text ?? ""; var name = (NameE.Text ?? "").Trim();
        if (!Regex.IsMatch(email, @"^\S+@\S+\.\S+$")) { Err.Text = "Enter a valid email address."; return; }
        if (pw.Length < 4) { Err.Text = "Password needs at least 4 characters."; return; }
        if (signUp && name == "") { Err.Text = "Enter your name."; return; }
        Go.IsEnabled = false;
        try { if (signUp) await Api.SignUp(name, email, pw); else await Api.SignIn(email, pw); App.GoMain(); }
        catch (Exception x) { Err.Text = x.Message; }
        finally { Go.IsEnabled = true; }
    }

    async void OnForgot(object? s, EventArgs e)
    {
        var email = await DisplayPromptAsync("Reset password", "Your account email:", initialValue: EmailE.Text, keyboard: Keyboard.Email);
        if (string.IsNullOrWhiteSpace(email)) return;
        var pw = await DisplayPromptAsync("Reset password", "New password (at least 4 characters):");
        if (pw is null || pw.Length < 4) { Err.Text = "New password needs at least 4 characters."; return; }
        try { await Api.ResetPassword(email.Trim().ToLower(), pw); await DisplayAlertAsync("Password updated", "Sign in with your new password.", "OK"); }
        catch (Exception x) { Err.Text = x.Message; }
    }
}
