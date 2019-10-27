namespace ImagePicker.Ios
{
    /// <summary>
    /// The source to use when picking an image or when determining available media types.
    /// Maps UIImagePickerControllerSourceType from UIKit.
    /// </summary>
    public enum IosImagePickerSourceType
    {
        /// <summary>
        /// Specifies the device’s photo library as the source for the image picker controller.
        /// </summary>
        PhotoLibrary = 0,
        
        /// <summary>
        /// Specifies the device’s built-in camera as the source for the image picker controller.
        /// Indicate the specific camera you want (front or rear, as available) by using the cameraDevice property.
        /// </summary>
        Camera = 1,
        
        /// <summary>
        /// Specifies the device’s Camera Roll album as the source for the image picker controller.
        /// If the device does not have a camera, specifies the Saved Photos album as the source.
        /// </summary>
        SavedPhotosAlbum = 2,
    }
}
