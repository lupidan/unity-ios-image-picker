namespace IosImagePicker.Enums
{
    /// <summary>
    /// The flash mode to use with the active camera.
    /// Maps UIImagePickerControllerCameraFlashMode from UIKit.
    /// </summary>
    public enum IosImagePickerCameraFlashMode
    {
        /// <summary>
        /// Specifies that flash illumination is always off, no matter what the ambient light conditions are.
        /// </summary>
        Off = -1,
        
        /// <summary>
        /// Specifies that the device should consider ambient light conditions to automatically determine whether or not to use flash illumination.
        /// </summary>
        Auto = 0,
        
        /// <summary>
        /// Specifies that flash illumination is always on, no matter what the ambient light conditions are.
        /// </summary>
        On = 1,
    }
}
