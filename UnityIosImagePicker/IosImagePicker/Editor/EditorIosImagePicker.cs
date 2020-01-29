using System;
using UnityEngine;

namespace IosImagePicker.Editor
{
    public class EditorIosImagePicker : IIosImagePicker
    {
        public bool IsSourceTypeAvailable(IosImagePickerSourceType sourceType)
        {
            return true;
        }

        public bool IsCameraDeviceAvailable(IosImagePickerCameraDevice cameraDevice)
        {
            return true;
        }

        public string[] AvailableMediaTypesForSourceType(IosImagePickerSourceType sourceType)
        {
            switch (sourceType)
            {
                case IosImagePickerSourceType.Camera:
                    return new[] {this.MediaTypeImage, this.MediaTypeMovie};
                case IosImagePickerSourceType.PhotoLibrary:
                    return new[] {this.MediaTypeImage};
                case IosImagePickerSourceType.SavedPhotosAlbum:
                    return new[] {this.MediaTypeMovie};
            }

            return null;
        }

        public IosImagePickerCameraCaptureMode[] AvailableCaptureModesForCameraDevice(IosImagePickerCameraDevice cameraDevice)
        {
            return new[] {IosImagePickerCameraCaptureMode.Photo, IosImagePickerCameraCaptureMode.Video};
        }

        public bool IsFlashAvailableForCameraDevice(IosImagePickerCameraDevice cameraDevice)
        {
            return cameraDevice == IosImagePickerCameraDevice.Rear;
        }

        public string MediaTypeImage => "public.image";
        public string MediaTypeMovie => "public.movie";
        public IosImagePickerSourceType SourceType { get; set; }
        public string[] MediaTypes { get; set; }
        public bool AllowsEditing { get; set; }
        public IosImagePickerVideoQualityType VideoQuality { get; set; }
        public TimeSpan VideoMaximumDuration { get; set; }
        public IosImagePickerCameraDevice CameraDevice { get; set; }
        public IosImagePickerCameraCaptureMode CameraCaptureMode { get; set; }
        public IosImagePickerCameraFlashMode CameraFlashMode { get; set; }
        public void Present(Action<IosImagePickerResult> resultCallback)
        {
            Debug.Log("PRESENTING CAMERA!");
        }

        public void Update()
        {
        }
    }
}