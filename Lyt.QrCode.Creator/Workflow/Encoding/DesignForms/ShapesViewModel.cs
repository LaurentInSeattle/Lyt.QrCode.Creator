namespace Lyt.QrCode.Creator.Workflow.Encoding.DesignForms;

public sealed partial class ShapesViewModel : ViewModel<ShapesView>
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel;

    public ShapesViewModel(QrCodeCreatorModel qrCodeCreatorModel)
    {
        this.qrCodeCreatorModel = qrCodeCreatorModel;
        this.ModuleShape = this.qrCodeCreatorModel.ModuleShape;
    }

    public override void Activate(object? activationParameters) 
    {
        base.Activate(activationParameters);
        this.OnModuleShapeChanged(this.ModuleShape);
    }

    [ObservableProperty]
    public partial ModuleShape ModuleShape { get; set; }

    partial void OnModuleShapeChanged(ModuleShape value) => this.qrCodeCreatorModel.SetModuleShape(value);
}
