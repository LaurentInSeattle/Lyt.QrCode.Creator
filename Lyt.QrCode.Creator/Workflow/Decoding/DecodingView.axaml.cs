namespace Lyt.QrCode.Creator.Workflow.Decoding;

public partial class DecodingView : View
{
    internal void AddDetectionSquare(double centerX, double centerY, double width, double angleRadians)
    {
        double angleDegrees = angleRadians * 180.0 / Math.PI; 
        var square = new Rectangle
        {
            Width = width,
            Height = width,
            Stroke = Brushes.Green,
            StrokeThickness = 4,
            RenderTransform = new RotateTransform(angleDegrees),
            RadiusX = 8, 
            RadiusY = 8,
        };

        Canvas.SetLeft(square, centerX - square.Width / 2);
        Canvas.SetTop(square, centerY - square.Height / 2);
        this.ImageCanvas.Children.Add(square);
    }

    internal void AddMarker(double x, double y)
    {
        var marker = new Ellipse
        {
            Width = 12,
            Height = 12,
            Fill = Brushes.Red,
            Stroke = Brushes.Black,
            StrokeThickness = 2
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

            if (control is Rectangle rectangle)
            {
                toRemove.Add(rectangle);
            }
        }

        this.ImageCanvas.Children.RemoveAll(toRemove);
    }
}
