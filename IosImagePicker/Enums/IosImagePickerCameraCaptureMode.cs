namespace IosImagePicker.Enums
{
    /// <summary>
    /// The category of media for the camera to capture.
    /// Maps UIImagePickerControllerCameraCaptureMode from UIKit
    /// </summary>
    public enum IosImagePickerCameraCaptureMode
    {
        /// <summary>
        /// Specifies that the camera captures still images.
        /// </summary>
        Photo = 0,
        
        /// <summary>
        /// Specifies that the camera captures movies.
        /// </summary>
        Video = 1,
    }
}
