namespace Lyt.QrCode.Creator.Workflow.Shared;

public partial class FormViewModel<TView, TValue> : 
    ViewModel<TView> where TView : View, new()
    where TValue : class, new()
{
    protected readonly QrCodeCreatorModel qrCodeCreatorModel;
    protected readonly IFormValidator<TValue> validator;

    public FormViewModel(QrCodeCreatorModel qrCodeCreatorModel, IFormValidator<TValue> validator)
    {
        this.qrCodeCreatorModel = qrCodeCreatorModel;
        this.validator = validator;
    }

    [ObservableProperty]
    public partial string ValidationMessage { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool FormIsValid { get; set; } = false;

    public override void OnViewLoaded()
    {
        base.OnViewLoaded();

        // Need to clear the form when the view gets loaded so that the focus will be set 
        this.validator.Clear(this);
    }

    protected void Submit(Action<TValue> submitAction)
    {
        try
        {
            if (!this.validator.Validate(this).IsValid)
            {
                return;
            }

            if (this.validator.HasValue)
            {
                var value = this.validator.Value;
                submitAction(value);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception thrown: {ex}");
        }
    }
}
