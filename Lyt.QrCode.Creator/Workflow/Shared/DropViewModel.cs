namespace Lyt.QrCode.Creator.Workflow.Shared;

public interface IDropImageTarget 
{
    void OnImageDrop(byte[] imageBytes);
}

public sealed partial class DropViewModel(IDropImageTarget dropImageTarget) : ViewModel<DropView>
{
    private readonly IDropImageTarget dropImageTarget = dropImageTarget;

    /// <summary> Returns true if the path is a valid image file. </summary>
    internal bool OnDrop(string path)
    {
        try
        {
            byte[] imageBytes = File.ReadAllBytes(path);
            if ((imageBytes is null) || (imageBytes.Length < 256))
            {
                throw new Exception("Failed to read image from disk: " + path);
            }

            var bitmap = WriteableBitmap.Decode(new MemoryStream(imageBytes));
            if (bitmap is not null)
            {
                this.dropImageTarget.OnImageDrop(imageBytes); 
                return true;
            }

            throw new Exception("Failed to load image: " + path);
        }
        catch (Exception ex)
        {
            this.Logger.Warning(ex.ToString());
            return false;
        }
    }
}
