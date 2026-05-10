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