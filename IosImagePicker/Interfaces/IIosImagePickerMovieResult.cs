namespace IosImagePicker.Interfaces
{
    public interface IIosImagePickerMovieResult
    {
        /// <summary>
        /// Movie file URL.
        /// Contains the file url returned by UIImagePickerControllerMediaURL.
        /// </summary>
        string MovieFileUrl { get; }
        
        /// <summary>
        /// Error if there was some problem while retrieving the movie file.
        /// </summary>
        IIosError MovieFileError { get; }
    }
}
