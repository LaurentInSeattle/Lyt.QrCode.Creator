namespace Lyt.QrCode.Creator.Workflow.Encoding;

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
    public partial ModulesViewModel ModulesViewModel { get; set; } = new(qrCodeCreatorModel);

    [ObservableProperty]
    public partial ShapesViewModel ShapesViewModel { get; set; } = new(qrCodeCreatorModel);

    [ObservableProperty]
    public partial OutputFormatViewModel OutputFormatViewModel { get; set; } = new(qrCodeCreatorModel);

    public override void OnViewLoaded()
    {
        base.OnViewLoaded();
        this.View.ContentContainer.ToggleCollapse();
        this.View.QrCodeContainer.ToggleCollapse();
    }

    [RelayCommand]
    public void OnNavigate(object? parameter)
    {
        if ((parameter is not string containerName) || string.IsNullOrWhiteSpace(containerName))
        {
            return;
        }

        // Navigate to the container for which we have a name
        Debug.WriteLine("Navigate to: " + containerName);
        var control = this.View.FindControl<ContainerControl>(containerName);
        if (control is ContainerControl containerControl)
        {
            var scrollViewer = this.View.ContainersScrollViewer;
            void ScrollTo()  => scrollViewer.Offset = new Vector(0, containerControl.Bounds.TopLeft.Y);
            if (containerControl.IsCollapsed)
            {
                // If we need to open the container, we need to schedule bringing it into view 
                // or else the scroll viewer will not show the collapsed section.
                containerControl.ToggleCollapse();
                Schedule.OnUiThread(66, ScrollTo, DispatcherPriority.Background); 
            }
            else
            {
                ScrollTo(); 
            }
        }        
    }
}