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

    public class StartDateValidator : AbstractValidator<DateTimeOffset?>
    {
        public StartDateValidator()
            => this.RuleFor(x => x)
                .NotNull().WithMessage("The event start date must be defined.")
                .GreaterThan(DateTime.Now.Date.AddDays(-1)).WithMessage("The event start date must be today or in the future.");
    }

    public class StartTimeValidator : AbstractValidator<TimeSpan?>
    {
        public StartTimeValidator()
            => this.RuleFor(x => x)
                .NotNull().WithMessage("The event start time must be defined.");
    }

    public class DurationValidator : AbstractValidator<TimeSpan?>
    {
        public DurationValidator()
            => this.RuleFor(x => x)
                .NotNull().WithMessage("The event duration must be defined.")
                .GreaterThan(TimeSpan.FromMinutes(1)).WithMessage("The event must last a least one minute.");
    }

    private static readonly FormValidator<CalendarEvent> CalendarEventValidator =
        new(focusFieldName: "SummaryTextBox",
            fieldValidators:
            [
                new FieldValidator<string> ("Summary", new SummaryStringValidator()),
                AlwaysValid<string>("Location"),
                AlwaysValid<string>("Description"),
                new FieldValidator<DateTimeOffset?>("StartDate", new StartDateValidator()),
                new FieldValidator<TimeSpan?>("StartTime", new StartTimeValidator()),
                new FieldValidator<TimeSpan?>("Duration", new DurationValidator()),
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
    public partial TimeSpan? StartTime { get; set; } = TimeSpan.FromHours(11.0);

    [ObservableProperty]
    public partial TimeSpan? Duration { get; set; } = TimeSpan.FromHours(1.0);

    [ObservableProperty]
    public partial bool IsAllDay { get; set; } = false;

    public override void OnViewLoaded()
    {
        base.OnViewLoaded();
        this.StartDate = DateTimeOffset.Now;
        this.StartTime = TimeSpan.FromHours(11.0);
        this.Duration = TimeSpan.FromHours(1.0);
    }

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
            if ( !value.StartDate.HasValue || !value.StartTime.HasValue || !value.Duration.HasValue)
            {
                return; 
            }

            DateTime startDate = value.StartDate.Value.Date;
            DateTime start = startDate  + value.StartTime.Value;
            DateTime end = value.IsAllDay ? startDate.AddDays(1) : start + value.Duration.Value;
            var content = new QrCalendarEvent(
                value.Summary,
                start, end, value.IsAllDay,
                value.Location, value.Description,
                includeVcalendarTags: true);
            if (!this.qrCodeCreatorModel.SetContent(content))
            {
                Debug.WriteLine("Failed to set content");
            }
        });
}
