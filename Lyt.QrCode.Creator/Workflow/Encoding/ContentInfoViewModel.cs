namespace Lyt.QrCode.Creator.Workflow.Encoding;

public sealed partial class ContentInfoViewModel(View view, string name, string iconName) : 
    ViewModel<ContentInfoView>
{
    public View TargetView { get; private set; } = view;

    [ObservableProperty]
    public partial string Name { get; private set; } = name;

    [ObservableProperty]
    public partial string IconName { get; private set; } = iconName;
}
