namespace IosImagePicker.Interfaces
{
    public interface IIosImagePickerResult
    {
        /// <summary>
        /// Whether the image picker was cancelled.
        /// </summary>
        bool DidCancel { get; }
        
        /// <summary>
        /// The media type that was returned by the native Image Picker.
        /// </summary>
        string MediaType { get; }
        
        /// <summary>
        /// If the user selected an image, it contains the data for that image.
        /// Otherwise it's null 
        /// </summary>
        IIosImagePickerImageResult Image { get; }
        
        /// <summary>
        /// If the user selected an video, it contains the data for that video.
        /// Otherwise it's null 
        /// </summary>
        IIosImagePickerMovieResult Movie { get; }
        
        /// <summary>
        /// If the image picker provided metadata for the media, this returns a string containing a JSON with all the metadata.
        /// If there is any binary data, a Base64 string of it is returned in the JSON.
        /// </summary>
        string MediaMetadataJson { get; }
    }
}
