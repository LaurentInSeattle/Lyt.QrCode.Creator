namespace Lyt.QrCode.Creator.Workflow.Decoding;

public partial class DecodingView : View
{
    internal void AddMarker(double x, double y)
    {
        var marker = new Ellipse
        {
            Width = 10,
            Height = 10,
            Fill = Brushes.Red,
            Stroke = Brushes.Black,
            StrokeThickness = 1
        };

        Canvas.SetLeft(marker, x - marker.Width / 2);
        Canvas.SetTop(marker, y - marker.Height / 2);
        this.ImageCanvas.Children.Add(marker);
    }

    internal void ClearMarkers()
    {
        List<Control> toRemove = [];
        foreach (var control in this.ImageCanvas.Children)
        {
            if (control is Ellipse ellipse)
            {
                toRemove.Add(ellipse);
            }
        }

        foreach (var control in toRemove)
        {
            this.ImageCanvas.Children.Remove(control);
        }
    }
}
