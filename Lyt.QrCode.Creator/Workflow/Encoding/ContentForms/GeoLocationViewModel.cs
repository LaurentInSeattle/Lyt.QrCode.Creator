namespace Lyt.QrCode.Creator.Workflow.Encoding.ContentForms;

public sealed partial class GeoLocationViewModel(QrCodeCreatorModel qrCodeCreatorModel) :
    FormViewModel<GeoLocationView, GeoLocationViewModel.GeoLocation>(qrCodeCreatorModel, GeoLocationValidator)
{
    public sealed record class GeoLocation(double Latitude = 0.0, double Longitude = 0.0)
    {
        public GeoLocation() : this(0.0 , 0.0) { }
    }

    private static readonly FormValidator<GeoLocation> GeoLocationValidator =
        new(
            new(
                FormValidPropertyName: "FormIsValid",
                MessagePropertyName: "ValidationMessage",
                FocusFieldName: "Latitude",
                FieldValidators: [Validators.Latitude, Validators.Longitude]));

    [ObservableProperty]
    public partial string Latitude { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Longitude { get; set; } = string.Empty;

    partial void OnLatitudeChanged(string value) => this.SubmitGeoLocation();

    partial void OnLongitudeChanged(string value) => this.SubmitGeoLocation();

    private void SubmitGeoLocation()
        => this.Submit(value =>
        {
            var content = new QrGeoLocation(value.Latitude, value.Longitude);
            if (!this.qrCodeCreatorModel.SetContent(content))
            {
                Debug.WriteLine("Failed to set content");
            }
        });
}
