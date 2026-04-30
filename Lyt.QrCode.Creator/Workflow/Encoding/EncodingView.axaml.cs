namespace Lyt.QrCode.Creator.Workflow.Encoding;

public partial class EncodingView : View
{
    private void OnNavigationButtonClick(object sender, RoutedEventArgs rea)
    {
        if (sender is not GlyphButton glyphButton)
        {
            return;
        }

        int maybeColumn = glyphButton.GetValue(Grid.ColumnProperty);
        if ((maybeColumn >= 1) && (maybeColumn <= 7)) 
        {
            this.NavigationIndicator.SetValue(Grid.ColumnProperty, maybeColumn);
        } 
    }
}

/*
  
 Debugging scrolling into view 

Add in aXAML : 			ScrollChanged="OnScrollChanged"
 
{
    private void OnScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        if (sender is not ScrollViewer scrollViewer)
        {
            return;
        }

        Debug.WriteLine(" --- Scroll changed --- ");
        Debug.WriteLine($"Offset: {scrollViewer.Offset}");
        Debug.WriteLine($"Extent: {scrollViewer.Extent}");
        Debug.WriteLine($"Viewport: {scrollViewer.Viewport}");
        var stackPanel = scrollViewer.FindChildControl<StackPanel>();
        if ( stackPanel is  null )
        {
            return;
        }

        foreach (var child in stackPanel.Children)
        {
            if (child is not ContainerControl containerControl)
            {
                continue;
            }

            Debug.WriteLine( containerControl.Name + " at: " + containerControl.Bounds);
        }
    }
}

*/ 