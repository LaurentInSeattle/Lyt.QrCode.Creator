namespace Lyt.QrCode.Creator.Controls;

public partial class OptionControl : UserControl
{
    public OptionControl() => this.InitializeComponent();

    protected override void OnLoaded(RoutedEventArgs e)
    {
        this.NoButton.IsChecked = true;
        this.ShowContent(show: false);
    }

    public static readonly StyledProperty<object?> OptionControlContentProperty =
        AvaloniaProperty.Register<OptionControl, object?>(nameof(OptionControlContent), null);

    public object? OptionControlContent
    {
        get => this.GetValue(OptionControlContentProperty);
        set
        {
            this.SetValue(OptionControlContentProperty, value);
            this.IfYesPresenter.Content = value;
            if (value is not null)
            {
                // A unique group name is required because there are multiple instance 
                // of this control under the same view.
                // We use the type name of the optional content view 
                string name = value.GetType().Name;
                this.YesButton.GroupName = name;
                this.NoButton.GroupName = name;
            }
        }
    }

    /// <summary> OptionText Styled Property </summary>
    public static readonly StyledProperty<string> OptionTextProperty =
        AvaloniaProperty.Register<OptionControl, string>(
            nameof(OptionText),
            defaultValue: string.Empty,
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: CoerceOptionText,
            enableDataValidation: false);

    /// <summary> Gets or sets the OptionText property.</summary>
    public string OptionText
    {
        get => this.GetValue(OptionTextProperty);
        set
        {
            this.SetValue(OptionTextProperty, value);
            this.OptionTextBlock.Text = value;
        }
    }

    /// <summary> Coerces the OptionText value. </summary>
    private static string CoerceOptionText(AvaloniaObject sender, string newText)
    {
        if (sender is OptionControl optionControl)
        {
            optionControl.OptionTextBlock.Text = newText;
        }

        return newText;
    }

    private void OnClickYes(object? __, RoutedEventArgs _) => this.ShowContent();

    private void OnClickNo(object? __, RoutedEventArgs _) => this.ShowContent(show: false);

    private void ShowContent(bool show = true)
    {
        if (this.IfYesPresenter.Content is UserControl userControl)
        {
            userControl.IsVisible = show;
            if (userControl.DataContext is ViewModel viewModel)
            {
                if (show)
                {
                    viewModel.Activate(null);
                }
                else
                {
                    viewModel.Deactivate();
                }
            }
        }

        var parent = MiscUtilities.FindParentControl<UserControl>(this); 
        parent?.InvalidateVisual();
    }
}