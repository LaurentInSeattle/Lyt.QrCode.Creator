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

    // Modules , scaling and colours

    public bool SetScale(int value) =>
        this.ApiAction(() =>
        {
            // TODO: Magic numbers
            if (value < 2 || value > 64)
            {
                return false;
            }

            this.Scale = value;
            return true;
        });

    public bool SetBorderSize(int value) =>
        this.ApiAction(() =>
        {
            // TODO: Magic numbers
            if (value < 2 || value > 16)
            {
                return false;
            }

            this.BorderSize = value;
            return true;
        });

    public bool SetLightColor(uint value) =>
        this.ApiAction(() =>
        {
            // TODO: Check luminance 
            this.FalseColor = value;
            return true;
        });

    public bool SetDarkColor(uint value) =>
        this.ApiAction(() =>
        {
            // TODO: Check luminance 
            this.TrueColor = value;
            return true;
        });

    // Frame

    public bool DoUseFrame(bool useFrame = true) =>
        this.ApiAction(() =>
        {
            this.UseFrame = useFrame;
            return true;
        });

    public bool SetFrameForegroundColor(uint color) =>
        this.ApiAction(() =>
        {
            if (!this.UseFrame)
            {
                return false;
            }

            this.FrameForegroundColor = color;
            return true;
        });

    public bool SetFrameSize(int value) =>
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

    public bool SetFrameBackgroundColor(uint color) =>
        this.ApiAction(() =>
        {
            if (!this.UseFrame)
            {
                return false;
            }

            this.FrameBackgroundColor = color;
            return true;
        });

    public bool SetFrameTextTop(string text) =>
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

    public bool SetFrameTextBottom(string text) =>
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

    public bool DoUseLogo(bool useLogo = true) =>
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

    public bool SetLogo(byte[] imageBytes) =>
        this.ApiAction(() =>
        {
            if (!this.UseLogo)
            {
                return false;
            }

            // TODO: Magic numbers
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

    public bool SetLogoSize(double logoSize) =>
        this.ApiAction(() =>
        {
            // TODO: Magic numbers
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

    public void UpdateOutputFilename(string filename) =>
        this.ApiAction(() =>
        {
            if ((string.IsNullOrWhiteSpace(filename)) ||
                (filename.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0))
            {
                return false;
            }

            this.OutputFileName = filename;

            // No need to publish an update message here.
            return false;
        });

    public void UpdateUseTimeStamp(bool useTimeStamp) =>
        this.ApiAction(() =>
        {
            this.UseTimeStamp = useTimeStamp;

            // No need to publish an update message here.
            return false;
        });

    public void UpdateOutputFormat(OutputFormat outputFormat) =>
        this.ApiAction(() =>
        {
            this.OutputFormat = outputFormat;

            // No need to publish an update message here.
            return false;
        });

    public void UpdateOutputLocation(OutputLocation outputLocation) =>
        this.ApiAction(() =>
        {
            this.OutputLocation = outputLocation;

            // No need to publish an update message here.
            return false;
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
