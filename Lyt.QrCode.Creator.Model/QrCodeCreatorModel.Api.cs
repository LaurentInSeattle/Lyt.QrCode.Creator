namespace Lyt.QrCode.Creator.Model;

using static Lyt.Persistence.FileManagerModel;

public sealed partial class QrCodeCreatorModel : ModelBase
{
    public bool IsDirty { get; private set; }

    public bool IsActive { get; private set; }

    public void GameIsActive(bool isActive = true) => this.IsActive = isActive;
}
