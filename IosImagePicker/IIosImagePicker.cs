using IosImagePicker.Enums;
using IosImagePicker.Interfaces;
using System;

namespace IosImagePicker
{
    public interface IIosImagePicker
    {
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
        IosImagePickerCameraCaptureMode CameraCaptureMode { get; set; }
        
        /// <summary>
        /// The flash mode used by the active camera.
        /// </summary>
        IosImagePickerCameraFlashMode CameraFlashMode { get; set; }

        /// <summary>
        /// Presents the IosImagePicker with a result callback.
        /// </summary>
        /// <param name="resultCallback">The callback to be called when the image pickers has finished.</param>
        void Present(Action<IIosImagePickerResult> resultCallback);

        /// <summary>
        /// Updates the picker controller to execute the result callbacks in a controlled context.
        /// </summary>
        void Update();

        /// <summary>
        /// Cleanup the temp plugin folder, and returns details about the cleanup.
        /// </summary>
        /// <param name="preview">If set to true, no deletion will happen. Use this to get a preview of what files would be deleted.</param>
        /// <returns>Result of the cleanup process with with all the details.</returns>
        IIosImagePickerCleanupResult CleanupPluginFolder(bool preview);
    }
}
