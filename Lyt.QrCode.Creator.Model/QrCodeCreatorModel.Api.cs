namespace Lyt.QrCode.Creator.Model;

public sealed partial class QrCodeCreatorModel : ModelBase
{
    public bool SetContent(QrContent qrContent) =>
        this.ApiAction(() =>
        {
            var result = Qr.EncodeToModules(qrContent);
            if (result.Success)
            {
                this.QrCodeContentType = qrContent.GetType();
                this.QrCodeContent = qrContent;
                this.Modules = result.Result;
                return true;
            }

            return false;
        });

    private bool ApiAction(Func<bool> action)
    {
        this.IsUpdatePending = true;
        this.timeoutTimer.ResetTimeout();
        return action();
    }
}
