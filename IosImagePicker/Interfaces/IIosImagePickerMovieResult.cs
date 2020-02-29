namespace IosImagePicker.Interfaces
{
    public interface IIosImagePickerMovieResult
    {
        /// <summary>
        /// Movie file path.
        /// On iOS, it contains the file path returned by UIImagePickerControllerMediaURL, copied in the temp folder.
        /// </summary>
        string MovieFilePath { get; }
        
        /// <summary>
        /// Error if there was some problem while retrieving the movie file.
        /// </summary>
        IIosError MovieFileError { get; }
    }
}
