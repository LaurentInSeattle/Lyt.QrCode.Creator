namespace Lyt.QrCode.Creator.Workflow.Encoding;

public sealed partial class ContentViewModel(QrCodeCreatorModel qrCodeCreatorModel) : ViewModel<ContentView>
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel = qrCodeCreatorModel;
    private View? currentSelectedView;
    private bool isInitializing;

    [ObservableProperty]
    public partial List<ContentInfoViewModel> SupportedContent { get; set; } = [];

    [ObservableProperty]
    public partial int SelectedContentIndex { get; set; }

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

    public override void OnViewLoaded()
    {
        base.OnViewLoaded();

        if (this.SupportedContent.Count == 0)
        {
            this.isInitializing = true;
            this.SupportedContent =
                [
                    new(this.View.UrlView, "Web Page Link (URL)", "preview_link") ,
                    new(this.View.WifiView, "Wi-Fi Configuration", "wifi_1") ,
                    new(this.View.MailView, "Email Address", "mail_read") ,
                    new(this.View.CalendarEventView, "iCal Calendar Event", "calendar") ,
                    new(this.View.MeCardView, "MeCard Contact Card", "contact_card") ,
                    new(this.View.VCardView, "VCard 4.0 Contact Card", "contact_card") ,
                    new(this.View.GeoLocationView, "Geo Location", "location") ,
                    new(this.View.PhoneNumberView, "Phone Number", "phone") ,
                    new(this.View.TextMessageView, "Text Message", "chat") ,
                    new(this.View.BookmarkView, "Web Page Bookmark", "bookmark") ,
                ];
            foreach (var content in this.SupportedContent)
            {
                content.TargetView.IsVisible = false;
            }

            this.isInitializing = false;

            // Force property changed so that we show something at first init 
            this.SelectedContentIndex = 1;
            this.SelectedContentIndex = 0;
        }
    }

    partial void OnSelectedContentIndexChanged(int value)
    {
        // Do not change content when initializing 
        if (this.isInitializing)
        {
            return;
        }

        if (this.currentSelectedView is not null)
        {
            this.currentSelectedView.IsVisible = false;
        }

        var selectedView = this.SupportedContent[value].TargetView;
        selectedView.IsVisible = true;
        this.currentSelectedView = selectedView;
    }
}