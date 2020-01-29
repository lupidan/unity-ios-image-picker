namespace IosImagePicker
{
    /// <summary>
    /// Video quality settings for movies recorded with the built-in camera, or transcoded by displaying in the image picker.
    /// Maps UIImagePickerControllerQualityType from UIKit
    /// </summary>
    public enum IosImagePickerVideoQualityType
    {
        /// <summary>
        /// If recording, specifies that you want to use the highest-quality video recording supported for the active camera on the device.
        /// </summary>
        High = 0,
        
        /// <summary>
        /// If recording, specifies that you want to use medium-quality video recording.
        /// </summary>
        Medium = 1,
        
        /// <summary>
        /// If recording, specifies that you want to use low-quality video recording.
        /// </summary>
        Low = 2,
        
        /// <summary>
        /// If recording, specifies that you want to use VGA-quality video recording (pixel dimensions of 640x480).
        /// </summary>
        VGA640x480 = 3,
        
        /// <summary>
        /// If recording, specifies that you want to use 1280x720 iFrame format.
        /// </summary>
        IFrame1280x720 = 4,
        
        /// <summary>
        /// If recording, specifies that you want to use 960x540 iFrame format.
        /// </summary>
        IFrame960x540 = 5,
    }
}
