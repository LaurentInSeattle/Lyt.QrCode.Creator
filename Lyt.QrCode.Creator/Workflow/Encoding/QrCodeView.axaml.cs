namespace Lyt.QrCode.Creator.Workflow.Encoding;

public partial class QrCodeView : View
{
    internal void ConstructGrid(
        bool[,] modules,
        int scale, int border, int frame,
        SolidColorBrush trueBrush, SolidColorBrush falseBrush,
        SolidColorBrush frameBackgroundBrush, SolidColorBrush frameForegroundBrush,
        string topText, string bottomText)
    {
        double screenScaling = App.MainWindow.Screens.ScreenFromVisual(this)?.Scaling ?? 1.0;
        double borderSize = border * scale / screenScaling;
        double moduleSize = scale / screenScaling;
        double frameSize = frame * scale / screenScaling;

        if (this.FrameGrid.FindChildControl<Grid>() is Grid oldGrid)
        {
            if (oldGrid.Name == "QrCodeGrid")
            {
                this.FrameGrid.Children.Remove(oldGrid);
            }
        }

        this.FrameGrid.Background = frameBackgroundBrush;
        var frameRows = this.FrameGrid.RowDefinitions;
        var frameCols = this.FrameGrid.ColumnDefinitions;
        frameRows[0].Height = new GridLength(frameSize, GridUnitType.Pixel);
        frameRows[2].Height = new GridLength(frameSize, GridUnitType.Pixel);
        frameCols[0].Width = new GridLength(frameSize, GridUnitType.Pixel);
        frameCols[2].Width = new GridLength(frameSize, GridUnitType.Pixel);

        if (frame == 0)
        {
            // If zero no frame
            this.TopTextBlock.Text = string.Empty;
            this.BottomTextBlock.Text = string.Empty;
        }
        else
        {
            this.TopTextBlock.Text = topText;
            this.BottomTextBlock.Text = bottomText;
            this.TopTextBlock.Foreground = frameForegroundBrush;
            this.BottomTextBlock.Foreground = frameForegroundBrush;
        }

        var grid = new Grid()
        {
            Name = "QrCodeGrid",
            Background = falseBrush,
        };

        int rows = modules.GetLength(0);
        int cols = modules.GetLength(1);
        grid.RowDefinitions.Add(new RowDefinition(borderSize, GridUnitType.Pixel));
        for (int i = 0; i < rows; i++)
        {
            grid.RowDefinitions.Add(new RowDefinition(moduleSize, GridUnitType.Pixel));
        }

        grid.RowDefinitions.Add(new RowDefinition(borderSize, GridUnitType.Pixel));

        grid.ColumnDefinitions.Add(new ColumnDefinition(borderSize, GridUnitType.Pixel));
        for (int j = 0; j < cols; j++)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition(moduleSize, GridUnitType.Pixel));
        }

        grid.ColumnDefinitions.Add(new ColumnDefinition(borderSize, GridUnitType.Pixel));

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                var rect = new Rectangle
                {
                    Fill = modules[i, j] ? trueBrush : falseBrush,
                    RadiusX = 0,
                    RadiusY = 0,
                    Stroke = Brushes.Transparent,
                    StrokeThickness = 0,
                };

                Grid.SetRow(rect, i + 1);
                Grid.SetColumn(rect, j + 1);
                grid.Children.Add(rect);
            }
        }

        Grid.SetColumn(grid, 1);
        Grid.SetRow(grid, 1);

        this.FrameGrid.Children.Add(grid);
        var centerRow = this.FrameGrid.RowDefinitions[1];
        var centerColumn = this.FrameGrid.ColumnDefinitions[1];
        int height = rows + border * 2;
        centerRow.Height = new GridLength(height * moduleSize, GridUnitType.Pixel);
        int width = cols + border * 2;
        centerColumn.Width = new GridLength(width * moduleSize, GridUnitType.Pixel);

        this.FrameGrid.Height = height * moduleSize + 2 * frameSize;
        this.FrameGrid.Width = width * moduleSize + 2 * frameSize;
        this.FrameGrid.InvalidateVisual();
    }
}
