using System.Text.Json;
namespace Wipey;

/// Local replacement for the Base44 client: accounts + three collections, stored on the phone.
public static class Api
{
    static T Get<T>(string k, T d) { var s = Preferences.Get(k, ""); return s == "" ? d : (JsonSerializer.Deserialize<T>(s) ?? d); }
    static void Set<T>(string k, T v) => Preferences.Set(k, JsonSerializer.Serialize(v));
    static Task Lag(int ms = 0) => Task.CompletedTask;   // artificial "network" delay removed: data is local, so it is instant

    // ---- auth ----
    public static User? Me => Get<User?>("user", null);
    public static async Task<User> SignUp(string name, string email, string pw)
    {
        await Lag(); var a = Get("accts", new Dictionary<string, Account>());
        if (a.ContainsKey(email)) throw new Exception("That email already has an account. Sign in instead.");
        a[email] = new Account { Pw = pw, Name = name }; Set("accts", a);
        var u = new User(email, name); Set("user", u); return u;
    }
    public static async Task<User> SignIn(string email, string pw)
    {
        await Lag(); var a = Get("accts", new Dictionary<string, Account>());
        if (!a.TryGetValue(email, out var acc)) throw new Exception("No account for this email. Create one below.");
        if (acc.Pw != pw) throw new Exception("Wrong password. Try again or reset it.");
        var u = new User(email, acc.Name); Set("user", u); return u;
    }
    public static async Task ResetPassword(string email, string pw)
    {
        await Lag(); var a = Get("accts", new Dictionary<string, Account>());
        if (!a.TryGetValue(email, out var acc)) throw new Exception("No account found for this email.");
        acc.Pw = pw; Set("accts", a);
    }
    public static async Task<User> UpdateProfile(string cur, string name, string email)
    {
        await Lag(150); var a = Get("accts", new Dictionary<string, Account>());
        if (email != cur && a.ContainsKey(email)) throw new Exception("That email is already in use.");
        var rec = a.GetValueOrDefault(cur) ?? new Account(); a.Remove(cur); rec.Name = name; a[email] = rec; Set("accts", a);
        var u = new User(email, name); Set("user", u); return u;
    }
    public static void SignOut() => Preferences.Remove("user");
    public static async Task DeleteAccount(string email)
    {
        var a = Get("accts", new Dictionary<string, Account>()); a.Remove(email); Set("accts", a);
        foreach (var c in Get("caps", new List<Capture>())) try { File.Delete(c.Path); } catch { }
        foreach (var k in new[] { "user", "logs", "caps", "cycles" }) Preferences.Remove(k);
        await Task.CompletedTask;
    }

    // ---- collections: "logs" (LogEntry), "caps" (Capture), "cycles" (Cycle) ----
    public static async Task<List<T>> List<T>(string key) { await Lag(100); return Get(key, new List<T>()); }
    public static async Task Save<T>(string key, List<T> items, int ms = 150) { await Lag(ms); Set(key, items); }
}
