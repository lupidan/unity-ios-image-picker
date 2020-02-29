using IosImagePicker.Interfaces;

namespace IosImagePicker.Editor
{
    public class EditorIosImagePickerResult : IIosImagePickerResult
    {
        public bool DidCancel { get; }
        public string MediaType { get; }
        public IIosImagePickerImageResult Image { get; }
        public IIosImagePickerMovieResult Movie { get; }
        public string MediaMetadataJson { get; }

        public EditorIosImagePickerResult(
            bool didCancel,
            string mediaType,
            EditorIosImagePickerImageResult image,
            EditorIosImagePickerMovieResult movie,
            string mediaMetadataJson)
        {
            DidCancel = didCancel;
            MediaType = mediaType;
            Image = image;
            Movie = movie;
            MediaMetadataJson = mediaMetadataJson;
        }
    }
}
