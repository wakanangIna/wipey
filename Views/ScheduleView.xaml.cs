namespace Wipey;

public partial class ScheduleView : ContentView
{
    List<Cycle> items = new();
    public ScheduleView() => InitializeComponent();
    public async Task OnShown() => await Load();
    async Task Load() { items = await Api.List<Cycle>("cycles"); List.ItemsSource = items; }

    // Optimistic update: the switch and the data change immediately; the "network" save happens after and rolls back on failure.
    async void OnToggle(object? s, ToggledEventArgs e)
    {
        if (s is not Switch sw || sw.BindingContext is not Cycle c || c.On == e.Value) return;
        var prev = c.On; c.On = e.Value;
        try { await Api.Save("cycles", items, 700); }
        catch { c.On = prev; sw.IsToggled = prev; await ConfirmSheet.Notice("Not saved", "Could not save. Change reverted."); }
    }

    async void OnDelete(object? s, TappedEventArgs e)
    {
        if ((s as BindableObject)?.BindingContext is not Cycle c) return;
        items.RemoveAll(x => x.Id == c.Id); await Api.Save("cycles", items); await Load();
    }

    async void OnAdd(object? s, EventArgs e)
    {
        var sheet = new AddCyclePage();
        await App.Nav.PushModalAsync(sheet, false);
        if (await sheet.Result.Task is not { } cycle) return;
        items.Insert(0, cycle);
        await Api.Save("cycles", items); await Load();
    }
}
