using System.Text.Json.Serialization;
namespace Wipey;

public record User(string Email, string Name);
public class Account { public string Pw { get; set; } = ""; public string Name { get; set; } = ""; }

public class LogEntry
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public DateTime At { get; set; } = DateTime.Now;
    [JsonIgnore] public string Day => At.ToString("dddd, MMM d");
    [JsonIgnore] public string Time => At.ToString("t");
}

public class Capture
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public DateTime At { get; set; } = DateTime.Now;
    public string Path { get; set; } = "";
    [JsonIgnore] public string Caption => At.ToString("ddd, MMM d, h:mm tt");
    [JsonIgnore] public string DateText => At.ToString("ddd, MMM d");
    [JsonIgnore] public string TimeText => At.ToString("h:mm tt");
}

public class Cycle
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public TimeSpan Time { get; set; } = new(18, 0, 0);
    public List<DayOfWeek> Days { get; set; } = new();   // days it runs; empty = older cycle, use Repeat/Dow below
    public string Repeat { get; set; } = "daily";        // legacy: daily | weekdays | weekly
    public bool On { get; set; } = true;
    public DayOfWeek Dow { get; set; } = DateTime.Today.DayOfWeek;   // legacy

    public bool RunsOn(DayOfWeek d) => Days.Count > 0 ? Days.Contains(d)
        : Repeat switch { "weekdays" => d is not (DayOfWeek.Saturday or DayOfWeek.Sunday), "weekly" => d == Dow, _ => true };

    [JsonIgnore] public string Label => (DateTime.Today + Time).ToString("t");
    [JsonIgnore] public string RepeatLabel
    {
        get
        {
            var order = new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday };
            var on = order.Where(RunsOn).ToList();
            if (on.Count == 7) return "Every day";
            if (on.Count == 5 && !RunsOn(DayOfWeek.Saturday) && !RunsOn(DayOfWeek.Sunday)) return "Weekdays";
            if (on.Count == 2 && RunsOn(DayOfWeek.Saturday) && RunsOn(DayOfWeek.Sunday)) return "Weekends";
            if (on.Count == 1) return "Every " + on[0];
            return on.Count == 0 ? "No days" : string.Join(", ", on.Select(d => d.ToString().Substring(0, 3)));
        }
    }
}
