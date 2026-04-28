namespace Lyt.QrCode.Creator.Model;

public sealed partial class QrCodeCreatorModel : ModelBase
{
    private bool SetContentInternal(QrContent qrContent, bool useLogo)
    {
        var encodeParameters = new EncodeParameters()
        {
            ErrorCorrectionLevel =
                useLogo ? EncodeParameters.QrErrorCorrectionLevel.High : EncodeParameters.QrErrorCorrectionLevel.Medium,
        };

        var result = Qr.EncodeToModules(qrContent, encodeParameters);
        if (result.Success)
        {
            this.QrCodeContentType = qrContent.GetType();
            this.QrCodeContent = qrContent;
            this.Modules = result.Result;
            return true;
        }

        return false;
    }

    public bool SetContent(QrContent qrContent) =>
        this.ApiAction(() =>
        {
            return this.SetContentInternal(qrContent, this.UseLogo);
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
            if (!this.UseFrame)
            {
                return false;
            }

            this.FrameForegroundColor = color;
            return true;
        });

    public void SetFrameSize(int value) =>
        this.ApiAction(() =>
        {
            if (!this.UseFrame)
            {
                return false;
            }

            if (value < 1 || value > 32)
            {
                return false;
            }

            this.FrameSize = value;
            return true;
        });

    public void SetFrameBackgroundColor(uint color) =>
        this.ApiAction(() =>
        {
            if (!this.UseFrame)
            {
                return false;
            }

            this.FrameBackgroundColor = color;
            return true;
        });

    public void SetFrameTextTop(string text) =>
        this.ApiAction(() =>
        {
            if (!this.UseFrame)
            {
                return false;
            }

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
            if (!this.UseFrame)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            this.FrameTextBottom = text;
            return true;
        });

    // Logo

    public void DoUseLogo(bool useLogo = true) =>
        this.ApiAction(() =>
        {
            bool result = true;
            if (this.UseLogo != useLogo)
            {
                // Rebuild the content, use the new value for UseLogo property 
                if (!this.SetContentInternal(this.QrCodeContent, useLogo))
                {
                    result = false;
                }

            }

            this.UseLogo = useLogo;
            return result;
        });

    public void SetLogo(byte[] imageBytes) =>
        this.ApiAction(() =>
        {
            if (!this.UseLogo)
            {
                return false;
            }

            if (imageBytes.Length < 256)
            {
                // Too small
                return false;
            }

            if (imageBytes.Length > 2 * 1024 * 1024)
            {
                // Too big
                return false;
            }

            this.LogoImageBytes = imageBytes;
            return true;
        });

    public void SetLogoSize(double logoSize) =>
        this.ApiAction(() =>
        {
            if (logoSize < 0.1)
            {
                // Too small
                return false;
            }

            if (logoSize > 0.35)
            {
                // Too big
                return false;
            }


            this.LogoSize = logoSize;
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
