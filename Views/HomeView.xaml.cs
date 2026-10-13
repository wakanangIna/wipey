namespace Wipey;

public partial class HomeView : ContentView
{
    List<Cycle> cycles = new(); bool erasing;
    public event Action<string>? GoTo;
    public HomeView() => InitializeComponent();

    public async Task OnShown() { SetGreeting(); await Load(); }

    public void SetGreeting()
    {
        var first = (Api.Me?.Name ?? "").Trim().Split(' ')[0]; if (first.Length > 0) first = char.ToUpper(first[0]) + first.Substring(1);
        Hi.Text = "Hi, " + first;
    }

    async Task Load()
    {
        cycles = await Api.List<Cycle>("cycles");
        var caps = await Api.List<Capture>("caps");
        var next = NextRun(); NextL.Text = next is null ? "Nothing scheduled" : next.Value.ToString("ddd, h:mm tt");
        var c = caps.FirstOrDefault();
        LastImg.Source = c is null ? null : ImageSource.FromFile(c.Path); LastImg.IsVisible = c is not null;
        LastL.Text = c is null ? "No photos yet" : c.At.ToString("MMM d");
    }

    DateTime? NextRun()
    {
        var now = DateTime.Now; DateTime? best = null;
        foreach (var c in cycles.Where(x => x.On))
            for (int i = 0; i < 8; i++)
            {
                var d = now.Date.AddDays(i) + c.Time; if (d <= now) continue; var w = d.DayOfWeek;
                if (c.RunsOn(w))
                { if (best is null || d < best) best = d; break; }
            }
        return best;
    }

    async void OnRefresh(object? s, EventArgs e) { await Load(); RV.IsRefreshing = false; }

    async void OnErase(object? s, EventArgs e)
    {
        if (erasing) return; erasing = true; EraseBtn.IsEnabled = false; Status.IsVisible = true; EraseBtn.Text = "Erasing\u2026";
        await Task.Delay(4000);   // replace with the real Bluetooth / Wi-Fi command to WIPEY
        var logs = await Api.List<LogEntry>("logs"); var entry = new LogEntry(); logs.Insert(0, entry); await Api.Save("logs", logs);
        erasing = false; EraseBtn.IsEnabled = true; EraseBtn.Text = "Erase now"; Status.Text = "Board erased at " + entry.Time;
    }

    void ToSchedule(object? s, TappedEventArgs e) => GoTo?.Invoke("schedule");
    void ToGallery(object? s, TappedEventArgs e) => GoTo?.Invoke("gallery");
}
