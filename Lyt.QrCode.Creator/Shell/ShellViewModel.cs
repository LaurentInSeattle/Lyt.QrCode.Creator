namespace Lyt.QrCode.Creator.Shell;

using static Messaging.ApplicationMessagingExtensions;

public sealed partial class ShellViewModel
    : ViewModel<ShellView>,
    IRecipient<ToolbarCommandMessage>,
    IRecipient<LanguageChangedMessage>
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel;
    private readonly IToaster toaster;

    [ObservableProperty]
    public partial bool MainToolbarIsVisible { get; set; }

    private ViewSelector<ActivatedView>? viewSelector;
    public bool isFirstActivation;

    public ShellViewModel(QrCodeCreatorModel qrCodeCreatorModel, IToaster toaster)
    {
        this.qrCodeCreatorModel = qrCodeCreatorModel;
        this.toaster = toaster;

        this.Subscribe<ToolbarCommandMessage>();
        this.Subscribe<LanguageChangedMessage>();
    }

    public void Receive(LanguageChangedMessage _)
    {
    }

    public void Receive(ToolbarCommandMessage message)
    {
        if (message.Command == ToolbarCommandMessage.ToolbarCommand.PlayFullscreen)
        {
        }
        else if (message.Command == ToolbarCommandMessage.ToolbarCommand.PlayWindowed)
        {
        }
    }

    public override void OnViewLoaded()
    {
        this.Logger.Debug("OnViewLoaded begins");

        base.OnViewLoaded();
        if (this.View is null)
        {
            throw new Exception("Failed to startup...");
        }

        // Select default language 
        string preferredLanguage = this.qrCodeCreatorModel.Language;
        this.Logger.Debug("Language: " + preferredLanguage);
        this.Localizer.SelectLanguage(preferredLanguage);
        Thread.CurrentThread.CurrentCulture = new CultureInfo(preferredLanguage);
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(preferredLanguage);

        this.Logger.Debug("OnViewLoaded language loaded");

        // Create all statics views and bind them 
        this.SetupWorkflow();
        this.Logger.Debug("OnViewLoaded SetupWorkflow complete");

        // Ready 
        this.toaster.Host = this.View.ToasterHost;
        this.toaster.Show(
            this.Localize("Shell.Ready"), this.Localize("Shell.Greetings"),
            5_000, InformationLevel.Info);

        this.isFirstActivation = true;
        // Select(this.qrCodeCreatorModel.IsFirstRun ? ActivatedView.Language : ActivatedView.Encoding);
        Select(ActivatedView.Encoding);

        this.Logger.Debug("OnViewLoaded complete");
    }

    private void SetupWorkflow()
    {
        if (this.View is not ShellView view)
        {
            throw new Exception("No view: Failed to startup...");
        }

        var selectableViews = new List<SelectableView<ActivatedView>>();

        void Setup<TViewModel, TControl, TToolbarViewModel, TToolbarControl>(
                ActivatedView activatedView, Control? control)
            where TViewModel : ViewModel<TControl>
            where TControl : Control, IView, new()
            where TToolbarViewModel : ViewModel<TToolbarControl>
            where TToolbarControl : Control, IView, new()
        {
            var vm = App.GetRequiredService<TViewModel>();
            vm.CreateViewAndBind();
            var vmToolbar = App.GetRequiredService<TToolbarViewModel>();
            vmToolbar.CreateViewAndBind();
            selectableViews.Add(
                new SelectableView<ActivatedView>(activatedView, vm, control, vmToolbar));
        }

        void SetupNoToolbar<TViewModel, TControl>(
                ActivatedView activatedView, Control control)
            where TViewModel : ViewModel<TControl>
            where TControl : Control, IView, new()
        {
            var vm = App.GetRequiredService<TViewModel>();
            vm.CreateViewAndBind();
            selectableViews.Add(new SelectableView<ActivatedView>(activatedView, vm, control));
        }

        SetupNoToolbar<EncodingViewModel, EncodingView>(ActivatedView.Encoding, view.EncodingButton);

        SetupNoToolbar<DecodingViewModel, DecodingView>(ActivatedView.Decoding, view.DecodingButton);

        Setup<LanguageViewModel, LanguageView, LanguageToolbarViewModel, LanguageToolbarView>(
            ActivatedView.Language, view.FlagButton);

        // Needs to be kept alive as a class member, or else callbacks will die (and wont work) 
        this.viewSelector =
            new ViewSelector<ActivatedView>(
                this.View.ShellViewContent,
                this.View.ShellViewToolbar,
                this.View.SelectionGroup,
                selectableViews,
                this.OnViewSelected);
    }

    private void OnViewSelected(ActivatedView activatedView)
    {
        if (this.viewSelector is null)
        {
            throw new Exception("No view selector");
        }

        var newViewModel = this.viewSelector.CurrentPrimaryViewModel;
        if (newViewModel is not null)
        {
            bool mainToolbarIsHidden = false;
            this.MainToolbarIsVisible = !mainToolbarIsHidden;
            this.Profiler.MemorySnapshot(
                newViewModel.ViewBase!.GetType().Name + ":  Activated", withGCCollect: false);
        }

        this.isFirstActivation = false;
    }

#pragma warning disable IDE0079 
#pragma warning disable CA1822 // Mark members as static

    [RelayCommand]
    public void OnEncoding() => Select(ActivatedView.Encoding);

    [RelayCommand]
    public void OnDecoding() => Select(ActivatedView.Decoding);

    [RelayCommand]
    public void OnLanguage() => Select(ActivatedView.Language);

    [RelayCommand]
    public void OnClose() => OnExit();

    private static async void OnExit()
    {
        var application = App.GetRequiredService<IApplicationBase>();
        await application.Shutdown();
    }
#pragma warning restore CA1822
#pragma warning restore IDE0079
}
