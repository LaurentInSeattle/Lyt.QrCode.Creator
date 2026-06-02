namespace Lyt.QrCode.Creator.Workflow.Encoding;

using global::Avalonia.Controls; // For 'Image', conflicting with one 'Lyt' namespace

using Lyt.Avalonia.Controls.Images;
using AvLayout = global::Avalonia.Layout; 

public partial class QrCodeView : View
{
    internal void ConstructGrid(
        bool[,] modules,
        int scale, int border, int frame,
        SolidColorBrush trueBrush, SolidColorBrush falseBrush,
        SolidColorBrush frameBackgroundBrush, SolidColorBrush frameForegroundBrush,
        string topText, string bottomText,
        int topTextFontSize, int bottomTextFontSize,
        int topTextFontWeight, int bottomTextFontWeight,
        FontFamily fontFamily,
        bool useLogo, byte[] logoImageBytes, double logoSize, int logoQuietZone,
        bool useBackground, byte[] backgroundImageBytes, double coloring, double darkModulesOpacity,
        ModuleShape moduleShape)
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

        this.FrameGrid.Background = Brushes.Transparent;
        var frameRows = this.FrameGrid.RowDefinitions;
        frameRows[0].Height = new GridLength(frameSize, GridUnitType.Pixel);
        frameRows[2].Height = new GridLength(frameSize, GridUnitType.Pixel);

        // Frame columns are made slightly smaller
        const double columnSizingFactor = 0.8;
        var frameCols = this.FrameGrid.ColumnDefinitions;
        frameCols[0].Width = new GridLength(frameSize * columnSizingFactor, GridUnitType.Pixel);
        frameCols[2].Width = new GridLength(frameSize * columnSizingFactor, GridUnitType.Pixel);

        if (frame == 0)
        {
            // If zero: no frame
            this.TopTextBlock.Text = string.Empty;
            this.BottomTextBlock.Text = string.Empty;
        }
        else
        {
            var frameRectangle = new Rectangle()
            {
                Fill = frameBackgroundBrush,
                RadiusX = moduleSize * 2,
                RadiusY = moduleSize * 2,
                HorizontalAlignment = AvLayout.HorizontalAlignment.Stretch,
                VerticalAlignment = AvLayout.VerticalAlignment.Stretch,
                ZIndex = -1,
            };

            Grid.SetColumn(frameRectangle, 0);
            Grid.SetRow(frameRectangle, 0);
            Grid.SetColumnSpan(frameRectangle, 3);
            Grid.SetRowSpan(frameRectangle, 3);
            this.FrameGrid.Children.Add(frameRectangle);

            this.TopTextBlock.Text = topText;
            this.TopTextBlock.Foreground = frameForegroundBrush;
            this.TopTextBlock.FontSize = topTextFontSize;
            this.TopTextBlock.FontWeight = (FontWeight)topTextFontWeight;
            this.TopTextBlock.FontFamily = fontFamily;

            this.BottomTextBlock.Text = bottomText;
            this.BottomTextBlock.Foreground = frameForegroundBrush;
            this.BottomTextBlock.FontSize = bottomTextFontSize;
            this.BottomTextBlock.FontWeight = (FontWeight)bottomTextFontWeight;
            this.BottomTextBlock.FontFamily = fontFamily;
        }

        var grid = new Grid()
        {
            Name = "QrCodeGrid",
            Background = Brushes.Transparent,
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


        Grid.SetColumn(grid, 1);
        Grid.SetRow(grid, 1);

        // Background image
        if (useBackground)
        {
            if (backgroundImageBytes is null || backgroundImageBytes.Length <= 250)
            {
                // No image ready yet: Add a coloured rectangle as placeholder
                var rectangle = new Rectangle()
                {
                    Fill = Brushes.DarkOrchid,
                    VerticalAlignment = AvLayout.VerticalAlignment.Stretch,
                    HorizontalAlignment = AvLayout.HorizontalAlignment.Stretch,
                };

                Grid.SetColumn(rectangle, 0);
                Grid.SetRow(rectangle, 0);
                Grid.SetColumnSpan(rectangle, cols + 2);
                Grid.SetRowSpan(rectangle, rows + 2);
                grid.Children.Add(rectangle);
            }
            else
            {
                using var ms = new MemoryStream(backgroundImageBytes);
                var bitmap = new Bitmap(ms);
                var image = new RoundedImage()
                {
                    Source = bitmap,
                    Stretch = Stretch.Fill,
                };

                image.SetValue(RoundedImage.CornerRadiusProperty, moduleSize * 1.2);

                Grid.SetColumn(image, 0);
                Grid.SetRow(image, 0);
                Grid.SetColumnSpan(image, cols + 2);
                Grid.SetRowSpan(image, rows + 2);
                grid.Children.Add(image);
            }

            // Add the coloring effect on top 
            var coloringRectangle = new Rectangle()
            {
                Fill = Brushes.White,
                RadiusX = moduleSize * 1.2,
                RadiusY = moduleSize * 1.2,
                Opacity = coloring,
                VerticalAlignment = AvLayout.VerticalAlignment.Stretch,
                HorizontalAlignment = AvLayout.HorizontalAlignment.Stretch,
            };

            Grid.SetColumn(coloringRectangle, 0);
            Grid.SetRow(coloringRectangle, 0);
            Grid.SetColumnSpan(coloringRectangle, cols + 2);
            Grid.SetRowSpan(coloringRectangle, rows + 2);
            grid.Children.Add(coloringRectangle);
        }
        else
        {
            var qrCodeRectangle = new Rectangle()
            {
                Fill = falseBrush,
                RadiusX = moduleSize * 1.2,
                RadiusY = moduleSize * 1.2,
                HorizontalAlignment = AvLayout.HorizontalAlignment.Stretch,
                VerticalAlignment = AvLayout.VerticalAlignment.Stretch,
            };

            Grid.SetColumn(qrCodeRectangle, 0);
            Grid.SetRow(qrCodeRectangle, 0);
            Grid.SetColumnSpan(qrCodeRectangle, cols + 2);
            Grid.SetRowSpan(qrCodeRectangle, rows + 2);
            grid.Children.Add(qrCodeRectangle);
        }

        // Add QR code modules above background image and below logo ( if any )
        Shape CreateModuleShape(int i, int j)
        {
            bool isDark = modules[i, j];
            var brush = isDark ? trueBrush : falseBrush;
            double opacity = isDark && useBackground ? darkModulesOpacity : 1.0;
            return moduleShape switch
            {
                ModuleShape.Square => new Rectangle()
                {
                    Fill = brush,
                    Opacity = opacity,
                    RadiusX = 0,
                    RadiusY = 0,
                    Stroke = Brushes.Transparent,
                    StrokeThickness = 0,
                },

                ModuleShape.Circle => new Ellipse()
                {
                    Fill = brush,
                    Opacity = opacity,
                    Height = moduleSize,
                    Width = moduleSize,
                },

                ModuleShape.RoundedSquare => new Rectangle()
                {
                    Fill = brush,
                    Height = moduleSize * 1.05,
                    Width = moduleSize * 1.05,
                    Opacity = opacity,
                    RadiusX = moduleSize / 4.0,
                    RadiusY = moduleSize / 4.0,
                    Stroke = Brushes.Transparent,
                    StrokeThickness = 0,
                },

                // Fails too often to decode 
                //ModuleShape.Diamond => new Polygon()
                //{
                //    Fill = brush,
                //    Margin = new Thickness(0),
                //    Points =
                //    [
                //        new(moduleSize / 2.0 + moduleSize / 4.0, 0),
                //        new(moduleSize / 2.0 - moduleSize / 4.0, 0),
                //        new (0, moduleSize / 4.0),
                //        new (moduleSize / 2.0 - 2.0, moduleSize + moduleSize / 8.0 ),
                //        new (moduleSize / 2.0 + 2.0, moduleSize + moduleSize / 8.0 ),
                //        new (moduleSize , moduleSize / 4.0 ),
                //    ]
                //},

                _ => throw new ArgumentOutOfRangeException(nameof(moduleShape), $"Unsupported module shape: {moduleShape}"),
            };
        }

        int pixelLogoSize = (int)Math.Round(rows * logoSize);

        bool IsLogoQuietZone(int i, int j)
        {
            if (!useLogo || logoQuietZone <= 0)
            {
                return false;
            }

            int centerCol = cols / 2;
            int centerRow = rows / 2;
            int halfLogoSize = pixelLogoSize / 2;
            return i >= centerRow - halfLogoSize - logoQuietZone &&
                   i <= centerRow + halfLogoSize + logoQuietZone &&
                   j >= centerCol - halfLogoSize - logoQuietZone &&
                   j <= centerCol + halfLogoSize + logoQuietZone;
        }

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (!modules[i, j])
                {
                    continue;
                }

                if (IsLogoQuietZone(i, j))
                {
                    continue;
                }

                var shape = CreateModuleShape(i, j);
                Grid.SetRow(shape, i + 1);
                Grid.SetColumn(shape, j + 1);
                grid.Children.Add(shape);
            }
        }

        // Logo on top of QR code and background image
        if (useLogo)
        {
            bool hasImage = logoImageBytes.Length > 0;
            var logoGrid = new Grid()
            {
                Name = "LogoGrid",
                Background = hasImage ? falseBrush : Brushes.HotPink,
            };

            // rows and cols in QR code are always odd, therefore size also must be
            // Parametrize logo size ( percent ) 
            int size = (int)Math.Round(rows * logoSize);
            if (size % 2 == 0)
            {
                // if even, make it odd 
                ++size;
            }

            int halfSize = size / 2;
            int centerCol = cols / 2;
            int startCol = 1 + centerCol - halfSize;
            Grid.SetRow(logoGrid, startCol);
            Grid.SetColumn(logoGrid, startCol);
            Grid.SetRowSpan(logoGrid, size);
            Grid.SetColumnSpan(logoGrid, size);
            grid.Children.Add(logoGrid);

            if (hasImage)
            {
                using var ms = new MemoryStream(logoImageBytes);
                var bitmap = new Bitmap(ms);
                var image = new RoundedImage()
                {
                    Source = bitmap,
                    Stretch = Stretch.Fill,
                };

                image.SetValue(RoundedImage.CornerRadiusProperty, moduleSize);
                logoGrid.Children.Add(image);
            }
        }

        this.FrameGrid.Children.Add(grid);
        var centerRow = this.FrameGrid.RowDefinitions[1];
        var centerColumn = this.FrameGrid.ColumnDefinitions[1];
        int height = rows + border * 2;
        centerRow.Height = new GridLength(height * moduleSize, GridUnitType.Pixel);
        int width = cols + border * 2;
        centerColumn.Width = new GridLength(width * moduleSize, GridUnitType.Pixel);

        this.FrameGrid.Height = height * moduleSize + 2.0 * frameSize;
        this.FrameGrid.Width = width * moduleSize + 2.0 * frameSize * columnSizingFactor;

        // TODO: Verify: Maybe not needed
        this.FrameGrid.InvalidateVisual();
    }
}
