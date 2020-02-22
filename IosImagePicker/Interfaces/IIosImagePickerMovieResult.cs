namespace IosImagePicker.Interfaces
{
    public interface IIosImagePickerMovieResult
    {
        /// <summary>
        /// Movie file URL.
        /// Contains the file url returned by UIImagePickerControllerMediaURL.
        /// </summary>
        string MovieFileUrl { get; }
    }
}
