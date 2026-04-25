namespace Lyt.QrCode.Creator.Workflow.Shared;

public partial class FormViewModel<TView, TValue>(
    QrCodeCreatorModel qrCodeCreatorModel, IFormValidator<TValue> validator) :
    ViewModel<TView> where TView : View, new()
    where TValue : class, new()
{
    protected readonly QrCodeCreatorModel qrCodeCreatorModel = qrCodeCreatorModel;
    protected readonly IFormValidator<TValue> validator = validator;

    private QrContent? lastValidContent; 

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

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);
        if (this.lastValidContent is not null)
        {
            this.qrCodeCreatorModel.SetContent(this.lastValidContent);
        }
    }

    protected void Submit(Func<TValue, QrContent?> submitAction)
    {
        try
        {
            if (!this.validator.Validate(this).IsValid)
            {
                Debug.WriteLine("Submit: Failed to validate content");
                return;
            }

            if (this.validator.HasValue)
            {
                var value = this.validator.Value;
                QrContent? content = submitAction(value);
                if (content is not null)
                {
                    if (!this.qrCodeCreatorModel.SetContent(content))
                    {
                        Debug.WriteLine("Submit: Failed to set content");
                        if (Debugger.IsAttached) { Debugger.Break(); }
                    }
                    else
                    {
                        Debug.WriteLine("Submit: Successfully set content");
                        this.lastValidContent = content;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Submit: Exception thrown: {ex}");
            if (Debugger.IsAttached) { Debugger.Break(); }
        }
    }
}
