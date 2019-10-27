using System;

namespace ImagePicker.Ios
{
    public interface IIosImagePicker
    {
        /// <summary>
        /// Returns a Boolean value indicating whether the device supports picking media using the specified source type.
        /// </summary>
        /// <param name="sourceType">The source to use to pick an image or movie.</param>
        /// <returns><c>true</c> if the device supports the specified source type, <c>false</c> if the specified source type is not available.</returns>
        bool IsSourceTypeAvailable(IosImagePickerSourceType sourceType);

        /// <summary>
        /// Returns a Boolean value that indicates whether a given camera is available.
        /// </summary>
        /// <param name="cameraDevice">A IosImagePickerCameraDevice constant indicating the camera whose availability you want to check.</param>
        /// <returns><c>true</c> if the camera indicated by cameraDevice is available, or <c>false</c> if it is not available.</returns>
        bool IsCameraDeviceAvailable(IosImagePickerCameraDevice cameraDevice);

        /// <summary>
        /// Returns an array of the available media types for the specified source type.
        /// </summary>
        /// <param name="sourceType">The source to use to pick an image.</param>
        /// <returns>An array whose elements identify the available media types for the specified source type.</returns>
        string[] AvailableMediaTypesForSourceType(IosImagePickerSourceType sourceType);

        /// <summary>
        /// Returns an array of IosImagePickerVideoCaptureMode indicating the capture modes supported by a given camera device.
        /// </summary>
        /// <param name="cameraDevice">A IosImagePickerCameraDevice constant indicating the camera you want to interrogate.</param>
        /// <returns>An array of IosImagePickerVideoCaptureMode indicating the capture modes supported by cameraDevice.</returns>
        IosImagePickerVideoCaptureMode[] AvailableCaptureModesForCameraDevice(IosImagePickerCameraDevice cameraDevice);

        /// <summary>
        /// Indicates whether a given camera has flash illumination capability.
        /// </summary>
        /// <param name="cameraDevice">A IosImagePickerCameraDevice constant indicating the camera whose flash capability you want to know.</param>
        /// <returns><c>true</c> if cameraDevice can use flash illumination, or <c>false</c> if it cannot.</returns>
        bool IsFlashAvailableForCameraDevice(IosImagePickerCameraDevice cameraDevice);
        
        /// <summary>
        /// The abstract media type identifier for image.
        /// </summary>
        string MediaTypeImage { get; }
        
        /// <summary>
        /// The abstract media type identifier for video.
        /// </summary>
        string MediaTypeMovie { get; }
        
        /// <summary>
        /// The type of picker interface to be displayed by the controller.
        /// </summary>
        IosImagePickerSourceType SourceType { get; set; }
        
        /// <summary>
        /// An array indicating the media types to be accessed by the media picker controller.
        /// </summary>
        string[] MediaTypes { get; set; }
        
        /// <summary>
        /// A Boolean value indicating whether the user is allowed to edit a selected still image or movie.
        /// </summary>
        bool AllowsEditing { get; set; }

        /// <summary>
        /// The video recording and transcoding quality.
        /// </summary>
        IosImagePickerVideoQualityType VideoQuality { get; set; }
        
        /// <summary>
        /// The maximum duration, in seconds, for a video recording.
        /// </summary>
        TimeSpan VideoMaximumDuration { get; set; }

        /// <summary>
        /// The camera used by the image picker controller.
        /// </summary>
        IosImagePickerCameraDevice CameraDevice { get; set; }
        
        /// <summary>
        /// The capture mode used by the camera.
        /// </summary>
        IosImagePickerVideoCaptureMode CameraCaptureMode { get; set; }
        
        /// <summary>
        /// The flash mode used by the active camera.
        /// </summary>
        IosImagePickerCameraFlashMode CameraFlashMode { get; set; }

        void Present();
    }
}
