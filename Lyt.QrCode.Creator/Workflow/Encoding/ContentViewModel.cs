namespace Lyt.QrCode.Creator.Workflow.Encoding;

public sealed partial class ContentViewModel(QrCodeCreatorModel qrCodeCreatorModel) : ViewModel<ContentView>
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel = qrCodeCreatorModel;

    [ObservableProperty]
    public partial BookmarkViewModel BookmarkViewModel { get; set; } = new(qrCodeCreatorModel);

    [ObservableProperty]
    public partial CalendarEventViewModel CalendarEventViewModel { get; set; } = new(qrCodeCreatorModel);

    [ObservableProperty]
    public partial GeoLocationViewModel GeoLocationViewModel { get; set; } = new(qrCodeCreatorModel);

    [ObservableProperty]
    public partial MailViewModel MailViewModel { get; set; } = new(qrCodeCreatorModel);

    [ObservableProperty]
    public partial MeCardViewModel MeCardViewModel { get; set; } = new(qrCodeCreatorModel);

    [ObservableProperty]
    public partial PhoneNumberViewModel PhoneNumberViewModel { get; set; } = new(qrCodeCreatorModel);

    [ObservableProperty]
    public partial TextMessageViewModel TextMessageViewModel { get; set; } = new(qrCodeCreatorModel);

    [ObservableProperty]
    public partial UrlViewModel UrlViewModel { get; set; } = new(qrCodeCreatorModel);

    [ObservableProperty]
    public partial VCardViewModel VCardViewModel { get; set; } = new(qrCodeCreatorModel);

    [ObservableProperty]
    public partial WifiViewModel WifiViewModel { get; set; } = new(qrCodeCreatorModel);
}