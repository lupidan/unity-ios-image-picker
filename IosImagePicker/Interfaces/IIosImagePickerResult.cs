namespace IosImagePicker.Interfaces
{
    public interface IIosImagePickerResult
    {
        bool DidCancel { get; }
        string MediaType { get; }
        IIosImagePickerImageResult Image { get; }
        IIosImagePickerMovieResult Movie { get; }
        string MediaMetadataJson { get; }
    }
}
