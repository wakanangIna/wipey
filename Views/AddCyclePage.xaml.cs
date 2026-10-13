namespace Wipey;

/// Bottom sheet for adding an erase cycle: pick a time and the days it repeats on.
public partial class AddCyclePage : ContentPage
{
    public TaskCompletionSource<Cycle?> Result { get; } = new();

    static readonly DayOfWeek[] Order = { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday };
    static readonly string[] Letters = { "M", "T", "W", "T", "F", "S", "S" };
    static readonly DayOfWeek[] WeekdaySet = { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };
    static readonly DayOfWeek[] WeekendSet = { DayOfWeek.Saturday, DayOfWeek.Sunday };

    readonly HashSet<DayOfWeek> picked = new(Order);   // default: every day
    readonly List<(DayOfWeek Day, Border Chip, Label Text)> chips = new();
    readonly List<(DayOfWeek[] Days, Border Chip, Label Text)> presets = new();

    static bool Dark => Preferences.Get("theme", "") is "dark" || (Preferences.Get("theme", "") == "" && Application.Current?.RequestedTheme == AppTheme.Dark);
    static Color Accent => Color.FromArgb(Dark ? "#7C8FF5" : "#5468E8");

    public AddCyclePage()
    {
        InitializeComponent();
        Picker.Time = new TimeSpan(18, 0, 0);
        ClockHost.Children.Add(Icons.Make(Icons.Clock, Color.FromArgb("#9AA0B4"), 20));

        for (int i = 0; i < 7; i++)
        {
            var day = Order[i];
            var text = new Label { Text = Letters[i], FontSize = 15, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
            var chip = new Border { WidthRequest = 40, HeightRequest = 40, Padding = 0, StrokeThickness = 1.5, StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 20 }, HorizontalOptions = LayoutOptions.Center, Content = text };
            chip.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => { if (!picked.Add(day)) picked.Remove(day); Refresh(); }) });
            DaysGrid.Add(chip, i, 0);
            chips.Add((day, chip, text));
        }

        foreach (var (name, days) in new[] { ("Every day", Order), ("Weekdays", WeekdaySet), ("Weekends", WeekendSet) })
        {
            var text = new Label { Text = name, FontSize = 13, FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center };
            var chip = new Border { Padding = new Thickness(12, 7), StrokeThickness = 1, StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 16 }, Content = text };
            var set = days;
            chip.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => { picked.Clear(); foreach (var d in set) picked.Add(d); Refresh(); }) });
            Presets.Add(chip);
            presets.Add((days, chip, text));
        }
        Refresh();
    }

    void Refresh()
    {
        var line = Color.FromArgb(Dark ? "#4A5064" : "#D5D8E0");
        var normal = Color.FromArgb(Dark ? "#F6F0E6" : "#2A2E3A");
        foreach (var (day, chip, text) in chips)
        {
            var on = picked.Contains(day);
            chip.BackgroundColor = on ? Accent : Colors.Transparent; chip.Stroke = new SolidColorBrush(on ? Accent : line);
            text.TextColor = on ? Colors.White : normal;
        }
        foreach (var (days, chip, text) in presets)
        {
            var on = picked.Count == days.Length && days.All(picked.Contains);
            chip.BackgroundColor = Colors.Transparent; chip.Stroke = new SolidColorBrush(on ? Accent : line);
            text.TextColor = on ? Accent : normal;
        }
        Summary.Text = "Repeats: " + new Cycle { Days = Order.Where(picked.Contains).ToList() }.RepeatLabel;
        Err.IsVisible = false;
    }

    protected override void OnDisappearing() { base.OnDisappearing(); Result.TrySetResult(null); }

    async void OnSave(object? s, EventArgs e)
    {
        if (picked.Count == 0) { Err.Text = "Pick at least one day."; Err.IsVisible = true; return; }
        TimeSpan time = (TimeSpan?)Picker.Time ?? new TimeSpan(18, 0, 0);
        Result.TrySetResult(new Cycle { Time = time, Days = Order.Where(picked.Contains).ToList(), Repeat = "custom" });
        await Navigation.PopModalAsync(false);
    }

    async void OnCancel(object? s, EventArgs e) => await Navigation.PopModalAsync(false);
}
