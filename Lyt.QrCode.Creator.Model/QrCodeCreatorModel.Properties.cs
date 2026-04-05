namespace Lyt.QrCode.Creator.Model;

public sealed partial class QrCodeCreatorModel : ModelBase
{
    #region Serialized -  No model changed event

    [JsonRequired]
    public string Language { get => this.Get<string>()!; set => this.Set(value); }

    /// <summary> This should stay true, ==> But... Just FOR NOW !  </summary>
    [JsonRequired]
    public bool IsFirstRun { get; set; } = false;

    #endregion Serialized -  No model changed event

    #region Not serialized - No model changed event

    // None for now 

    #endregion Not serialized - No model changed event

    #region NOT serialized - WITH model changed event

    // None for now 

    #endregion NOT serialized - WITH model changed event    
}
