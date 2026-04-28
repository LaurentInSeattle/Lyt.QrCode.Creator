namespace Lyt.QrCode.Creator.Workflow.Encoding;

using Lyt.QrCode.Creator.Controls;

public sealed partial class EncodingViewModel(QrCodeCreatorModel qrCodeCreatorModel) : ViewModel<EncodingView>
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel = qrCodeCreatorModel;

    [ObservableProperty]
    public partial QrCodeViewModel QrCodeViewModel { get; set; } = new(qrCodeCreatorModel);

    [ObservableProperty]
    public partial TestImageViewModel TestImageViewModel { get; set; } = new(qrCodeCreatorModel);

    [ObservableProperty]
    public partial ContentViewModel ContentViewModel { get; set; } = new(qrCodeCreatorModel);

    [ObservableProperty]
    public partial FrameViewModel FrameViewModel { get; set; } = new(qrCodeCreatorModel);

    [ObservableProperty]
    public partial LogoViewModel LogoViewModel { get; set; } = new(qrCodeCreatorModel);

    [ObservableProperty]
    public partial ImageViewModel ImageViewModel { get; set; } = new(qrCodeCreatorModel);

    [ObservableProperty]
    public partial ColorsViewModel ColorsViewModel { get; set; } = new(qrCodeCreatorModel);

    [ObservableProperty]
    public partial ShapesViewModel ShapesViewModel { get; set; } = new(qrCodeCreatorModel);

    [ObservableProperty]
    public partial SizeFormatViewModel SizeFormatViewModel { get; set; } = new(qrCodeCreatorModel);

    public override void OnViewLoaded()
    {
        base.OnViewLoaded();
        this.View.ContentContainer.ToggleCollapse();
        this.View.QrCodeContainer.ToggleCollapse();
    }

    [RelayCommand]
    public void OnNavigate(object? parameter)
    {
        if ((parameter is string containerName) && !string.IsNullOrWhiteSpace(containerName))
        {
            // TODO: Navigate 
            Debug.WriteLine("Navigate to: " + containerName);
            var control = this.View.FindControl<ContainerControl>(containerName);
            if (control is ContainerControl containerControl)
            {
                if (containerControl.IsCollapsed)
                {
                    // If we need to open the container, we need to schedule bringing it into view 
                    // or else the scroll viewer will not show the collapsed section.
                    containerControl.ToggleCollapse();
                    Schedule.OnUiThread(66, containerControl.BringIntoView, DispatcherPriority.Background); 
                }
                else
                {
                    containerControl.BringIntoView();
                }
            }
        }
    }
}