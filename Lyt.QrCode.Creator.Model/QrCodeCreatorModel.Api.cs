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

    // Module colors
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

    // Frame

    public void DoUseFrame(bool useFrame = true) =>
        this.ApiAction(() =>
        {
            this.UseFrame = useFrame;
            return true;
        });

    public void SetFrameForegroundColor(uint color) =>
        this.ApiAction(() =>
        {
            this.FrameForegroundColor = color;
            return true;
        });

    public void SetFrameBackgroundColor(uint color) =>
        this.ApiAction(() =>
        {
            this.FrameBackgroundColor = color;
            return true;
        });

    public void SetFrameTextTop(string text) =>
        this.ApiAction(() =>
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return false; 
            }

            this.FrameTextTop = text;
            return true;
        });

    public void SetFrameTextBottom(string text) =>
        this.ApiAction(() =>
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            this.FrameTextBottom = text;
            return true;
        });


    public void DoUseLogo(bool useLogo = true) =>
        this.ApiAction(() =>
        {
            this.UseLogo = useLogo;
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
