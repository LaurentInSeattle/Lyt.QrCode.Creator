namespace Lyt.QrCode.Creator.Workflow.Encoding.ContentForms;

using static Lyt.QrCode.Creator.Model.Validation.Validators;

public sealed partial class CalendarEventViewModel(QrCodeCreatorModel qrCodeCreatorModel) :
    FormViewModel<CalendarEventView, CalendarEventViewModel.CalendarEvent>(qrCodeCreatorModel, CalendarEventValidator)
{
    public sealed record class CalendarEvent(
        string Summary,
        string Location = "", 
        string Description = "",
        DateTimeOffset? StartDate = default,
        TimeSpan? StartTime = default,
        TimeSpan? Duration = default, 
        bool IsAllDay = false )
    {
        public CalendarEvent() : 
            this(string.Empty, string.Empty, string.Empty, 
                DateTime.Now, TimeSpan.Zero, TimeSpan.Zero, false) { }
    }

    public class SummaryStringValidator : AbstractValidator<string>
    {
        public SummaryStringValidator()
            => this.RuleFor(x => x)
                .NotEmpty().WithMessage("The event summary cannot be empty.")
                .MinimumLength(4).WithMessage("The event summary is too short.")
                .MaximumLength(60).WithMessage("The event summary is too long.");
    }

    private static readonly FieldValidator<string> SummaryValidator =
        new(validator: new SummaryStringValidator(), sourcePropertyName: "Summary");

    private static readonly FormValidator<CalendarEvent> CalendarEventValidator =
        new(focusFieldName: "SummaryTextBox",
            fieldValidators:
            [
                SummaryValidator,
                AlwaysValid<string>("Location"),
                AlwaysValid<string>("Description"),
                AlwaysValid<DateTimeOffset?>("StartDate"),
                AlwaysValid<TimeSpan?>("StartTime"),
                AlwaysValid<TimeSpan?>("Duration"),
                AlwaysValid<bool>("IsAllDay"),
            ]);


    [ObservableProperty]
    public partial string Summary { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Location { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Description { get; set; } = string.Empty;

    [ObservableProperty]
    public partial DateTimeOffset? StartDate { get; set; } = DateTime.Now;

    [ObservableProperty]
    public partial TimeSpan? StartTime { get; set; } = TimeSpan.Zero;

    [ObservableProperty]
    public partial TimeSpan? Duration { get; set; } = TimeSpan.Zero;

    [ObservableProperty]
    public partial bool IsAllDay { get; set; } = false;

    partial void OnSummaryChanged(string value) => this.SubmitCalendarEvent();

    partial void OnLocationChanged(string value) => this.SubmitCalendarEvent();

    partial void OnDescriptionChanged(string value) => this.SubmitCalendarEvent();

    partial void OnStartDateChanged(DateTimeOffset? value) => this.SubmitCalendarEvent();

    partial void OnStartTimeChanged(TimeSpan? value) => this.SubmitCalendarEvent();

    partial void OnDurationChanged(TimeSpan? value) => this.SubmitCalendarEvent();

    partial void OnIsAllDayChanged(bool value) => this.SubmitCalendarEvent();

    private void SubmitCalendarEvent()
        => this.Submit(value =>
        {
            //var start = value.StartDate.Value.Date + value.StartTime;
            //var end = value.IsAllDay ? start.Date.AddDays(1) : start + value.Duration;
            //var content = new QrCalendarEvent(
            //    value.Summary, 
            //    start, end, value.IsAllDay, 
            //    value.Location, value.Description, 
            //    includeVcalendarTags:true);
            //if (!this.qrCodeCreatorModel.SetContent(content))
            //{
            //    Debug.WriteLine("Failed to set content");
            //}
        });
}
