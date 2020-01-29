namespace IosImagePicker
{
    /// <summary>
    /// The camera to use for image or movie capture.
    /// Maps UIImagePickerControllerCameraDevice from UIKit.
    /// </summary>
    public enum IosImagePickerCameraDevice
    {
        /// <summary>
        /// Specifies the camera on the rear of the device.
        /// </summary>
        Rear = 0,
        
        /// <summary>
        /// Specifies the camera on the front of the device.
        /// </summary>
        Front = 1,
    }
}
