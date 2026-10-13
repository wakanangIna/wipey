namespace Wipey;
public partial class LogView : ContentView
{
    public LogView() => InitializeComponent();
    public async Task OnShown() => List.ItemsSource = await Api.List<LogEntry>("logs");
}
