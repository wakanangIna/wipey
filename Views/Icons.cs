using Microsoft.Maui.Controls.Shapes;
using Ico = Microsoft.Maui.Controls.Shapes.Path;
namespace Wipey;

/// Line icons drawn as vector paths (no image files or fonts needed).
public static class Icons
{
    public const string Home     = "M3 11 L12 3 L21 11 M5 9.5 V20 H10 V14 H14 V20 H19 V9.5";
    public const string Gallery  = "M4 5 H20 V19 H4 Z M4 16 L9 11 L13 15 M13 13 L16 10 L20 14 M7.5 8.5 A1 1 0 1 0 9.5 8.5 A1 1 0 1 0 7.5 8.5";
    public const string Calendar = "M4 6 H20 V20 H4 Z M4 10 H20 M8 3 V7 M16 3 V7";
    public const string Check    = "M3 12 A9 9 0 1 0 21 12 A9 9 0 1 0 3 12 M8 12.5 L11 15.5 L16 9.5";
    public const string Camera   = "M4 8 H8 L10 5 H14 L16 8 H20 V19 H4 Z M9 13 A3 3 0 1 0 15 13 A3 3 0 1 0 9 13";
    public const string Clock    = "M3 12 A9 9 0 1 0 21 12 A9 9 0 1 0 3 12 M12 7 V12 L15 14";
    public const string Tick     = "M5 12.5 L10 17.5 L19 7";
    public const string Close    = "M6 6 L18 18 M18 6 L6 18";
    public const string Moon     = "M20 14 A8 8 0 1 1 10 4 A6.5 6.5 0 0 0 20 14 Z";
    public const string Sun      = "M8 12 A4 4 0 1 0 16 12 A4 4 0 1 0 8 12 M12 2 V4 M12 20 V22 M4.9 4.9 L6.3 6.3 M17.7 17.7 L19.1 19.1 M2 12 H4 M20 12 H22 M4.9 19.1 L6.3 17.7 M17.7 6.3 L19.1 4.9";

    public static Ico Make(string data, Color color, double size = 22, double thickness = 2) => new Ico
    {
        Data = (Geometry)new PathGeometryConverter().ConvertFromInvariantString(data)!,
        Stroke = new SolidColorBrush(color),
        StrokeThickness = thickness,
        StrokeLineCap = PenLineCap.Round,
        StrokeLineJoin = PenLineJoin.Round,
        Aspect = Stretch.Uniform,
        WidthRequest = size, HeightRequest = size,
        HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center,
    };
}
