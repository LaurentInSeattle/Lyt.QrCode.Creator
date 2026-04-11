namespace Lyt.QrCode.Creator.Workflow.Shared;

public partial class FormViewModel<TView> : ViewModel<TView> where TView : View, new()
{
    protected readonly QrCodeCreatorModel qrCodeCreatorModel;

    public FormViewModel(QrCodeCreatorModel qrCodeCreatorModel)
    {
        this.qrCodeCreatorModel = qrCodeCreatorModel;
    }

    [ObservableProperty]
    public partial string ValidationMessage { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool FormIsValid { get; set; } = false;

}
