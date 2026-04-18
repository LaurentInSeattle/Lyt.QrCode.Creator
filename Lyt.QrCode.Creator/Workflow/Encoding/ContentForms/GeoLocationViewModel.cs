namespace Lyt.QrCode.Creator.Workflow.Encoding.ContentForms;

using static Lyt.QrCode.Content.QrGeoLocation;
using static Lyt.QrCode.Creator.Model.Validation.Validators;

public sealed partial class GeoLocationViewModel(QrCodeCreatorModel qrCodeCreatorModel) :
    FormViewModel<GeoLocationView, GeoLocationViewModel.GeoLocation>(qrCodeCreatorModel, GeoLocationValidator)
{
    public sealed record class GeoLocation(
        double Latitude = 0.0, double Longitude = 0.0, GeoProtocol Protocol = GeoProtocol.Geo)
    {
        public GeoLocation() : this(0.0, 0.0) { }
    }

    private static readonly FormValidator<GeoLocation> GeoLocationValidator =
         new([Validators.Latitude, Validators.Longitude, AlwaysValid<GeoProtocol>("Protocol")],
             focusFieldName: "Latitude");

    [ObservableProperty]
    public partial string Latitude { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Longitude { get; set; } = string.Empty;

    [ObservableProperty]
    public partial GeoProtocol Protocol { get; set; } = GeoProtocol.Geo;

    public override void OnViewLoaded()
    {
        this.Protocol = GeoProtocol.Geo;
        this.View.GeoProtocolButton.IsChecked = true; 
    } 

    partial void OnLatitudeChanged(string value) => this.SubmitGeoLocation();

    partial void OnLongitudeChanged(string value) => this.SubmitGeoLocation();

    partial void OnProtocolChanged(GeoProtocol value) => this.SubmitGeoLocation();

    private void SubmitGeoLocation()
        => this.Submit(value => new QrGeoLocation(value.Latitude, value.Longitude, value.Protocol));
}
