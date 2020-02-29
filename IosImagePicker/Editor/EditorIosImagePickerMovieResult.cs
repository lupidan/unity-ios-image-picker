using IosImagePicker.Interfaces;

namespace IosImagePicker.Editor
{
    public class EditorIosImagePickerMovieResult : IIosImagePickerMovieResult
    {
        public string MovieFilePath { get; }
        public IIosError MovieFileError { get; }

        public EditorIosImagePickerMovieResult(
            string movieFilePath,
            EditorIosError movieFileError)
        {
            MovieFilePath = movieFilePath;
            MovieFileError = movieFileError;
        }
    }
}
