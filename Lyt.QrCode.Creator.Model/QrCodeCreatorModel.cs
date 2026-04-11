namespace Lyt.QrCode.Creator.Model;

using Lyt.Framework.Interfaces.Profiling;
using Lyt.QrCode.Creator.Model.Utilities;

using static Lyt.Persistence.FileManagerModel;

public sealed partial class QrCodeCreatorModel : ModelBase
{
    public const string DefaultLanguage = "fr-FR";
    private const string JigsawModelFilename = "QrCodeCreatorData";

    private static readonly QrCodeCreatorModel DefaultData =
        new()
        {
            Language = DefaultLanguage,
            IsFirstRun = true,
        };

    private readonly FileManagerModel fileManager;
    private readonly ILocalizer localizer;
    private readonly IProfiler profiler; 
    private readonly FileId modelFileId;
    private readonly TimeoutTimer timeoutTimer;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
    public QrCodeCreatorModel() : base(null)
    {
        this.modelFileId = new FileId(Area.User, Kind.Json, QrCodeCreatorModel.JigsawModelFilename);
        // Do not inject the FileManagerModel instance: a parameter-less ctor is required for Deserialization 
        // Empty CTOR required for deserialization 
        this.ShouldAutoSave = false;
    }
#pragma warning restore CS8625 
#pragma warning restore CS8618

    public QrCodeCreatorModel(
        FileManagerModel fileManager,
        ILocalizer localizer,
        IProfiler profiler,
        ILogger logger) : base(logger)
    {
        this.fileManager = fileManager;
        this.localizer = localizer;
        this.profiler = profiler;
        this.modelFileId = new FileId(Area.User, Kind.Json, QrCodeCreatorModel.JigsawModelFilename);
        this.timeoutTimer = new TimeoutTimer(this.OnModelUpdate, timeoutMilliseconds: 250);
        this.ShouldAutoSave = true;
    }

    public override async Task Initialize()
    {
        this.IsInitializing = true;
        await this.Load();
        this.IsInitializing = false;
        this.IsDirty = false;
    }

    public override async Task Shutdown()
    {
        // Force a save on shutdown 
        await this.Save();
    }

    public Task Load()
    {
        try
        {
            if (!this.fileManager.Exists(this.modelFileId))
            {
                this.fileManager.Save(this.modelFileId, QrCodeCreatorModel.DefaultData);
            }

            QrCodeCreatorModel model = this.fileManager.Load<QrCodeCreatorModel>(this.modelFileId);

            // Copy all properties with attribute [JsonRequired]
            base.CopyJSonRequiredProperties<QrCodeCreatorModel>(model);

            new ModelLoadedMessage().Publish();
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            string msg = "Failed to load Model from " + this.modelFileId.Filename;
            this.Logger.Fatal(msg);
            throw new Exception("", ex);
        }
    }

    public override Task Save()
    {
        // Null check is needed !
        // If the File Manager is null we are currently loading the model and activating properties on a second instance 
        // causing dirtyness, and in such case we must avoid the null crash and anyway there is no need to save anything.
        if (this.fileManager is not null)
        {
#if DEBUG 
            //if (this.fileManager.Exists(this.modelFileId))
            //{
            //    this.fileManager.Duplicate(this.modelFileId);
            //}
#endif // DEBUG 

            this.fileManager.Save(this.modelFileId, this);

#if DEBUG 
            //try
            //{
            //    string path = this.fileManager.MakePath(this.modelFileId);
            //    var fileInfo = new FileInfo(path);
            //    if (fileInfo.Length < 1024)
            //    {
            //        // if (Debugger.IsAttached) { Debugger.Break(); }
            //        this.Logger.Warning("Model file is too small!");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    if (Debugger.IsAttached) { Debugger.Break(); }
            //    Debug.WriteLine(ex);
            //}
#endif // DEBUG 

            base.Save();
        }

        return Task.CompletedTask;
    }

    public void SelectLanguage(string languageKey)
    {
        this.Language = languageKey;
        this.localizer.SelectLanguage(languageKey);
    }

    public void ClearFirstRun()
    {
        this.IsFirstRun = false;
        this.Save();
    }

    private void OnModelUpdate()
    {
        if (!this.IsUpdatePending)
        {
            return;
        }

        this.IsUpdatePending = false; 
        new ModelChangedMessage().Publish();
    }
}
