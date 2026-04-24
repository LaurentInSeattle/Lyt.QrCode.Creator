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

    public void SetLightColor(uint value) =>
        this.ApiAction(() =>
        {
            // TODO: Check luminance 
            this.FalseColor = value;
            return true;
        });

    public void SetDarkColor(uint value) =>
        this.ApiAction(() =>
        {
            // TODO: Check luminance 
            this.TrueColor = value;
            return true;
        });

    private bool ApiAction(Func<bool> action)
    {
        if (!this.timeoutTimer.IsRunning)
        {
            this.timeoutTimer.Start();
        }

        this.timeoutTimer.ResetTimeout();
        bool success = action();
        if (success)
        {
            this.IsUpdatePending = true;
        }

        return success;
    }
}
