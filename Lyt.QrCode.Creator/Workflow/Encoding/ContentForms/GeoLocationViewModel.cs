namespace Lyt.QrCode.Creator.Workflow.Encoding.ContentForms;

public sealed partial class GeoLocationViewModel : ViewModel<GeoLocationView>
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel;

    public GeoLocationViewModel(QrCodeCreatorModel qrCodeCreatorModel)
    {
        this.qrCodeCreatorModel = qrCodeCreatorModel;
    }
}
