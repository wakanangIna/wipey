namespace Wipey;

public partial class GalleryView : ContentView
{
    List<Capture> items = new();
    public GalleryView() => InitializeComponent();
    public async Task OnShown() => await Load();
    async Task Load() { items = await Api.List<Capture>("caps"); List.ItemsSource = items; }
    async void OnRefresh(object? s, EventArgs e) { await Load(); RV.IsRefreshing = false; }

    async void OnPick(object? s, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Capture c) return;
        List.SelectedItem = null;
        if (!await ConfirmSheet.Ask("Delete photo?", c.Caption, "Delete", "Keep", danger: true, picture: ImageSource.FromFile(c.Path))) return;
        try { File.Delete(c.Path); } catch { }
        items.RemoveAll(x => x.Id == c.Id); await Api.Save("caps", items); await Load();
    }
}
