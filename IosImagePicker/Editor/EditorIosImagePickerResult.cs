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
            this.DidCancel = didCancel;
            this.MediaType = mediaType;
            this.Image = image;
            this.Movie = movie;
            this.MediaMetadataJson = mediaMetadataJson;
        }
    }
}
