namespace Lyt.QrCode.Creator.Workflow.Encoding.ContentForms;

public sealed partial class CalendarEventViewModel : ViewModel<CalendarEventView>
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel;

    public CalendarEventViewModel(QrCodeCreatorModel qrCodeCreatorModel)
    {
        this.qrCodeCreatorModel = qrCodeCreatorModel;
    }
}
