using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ImagePicker.Ios
{
    public class IosImagePicker : IIosImagePicker
    {
        public bool IsSourceTypeAvailable(IosImagePickerSourceType sourceType)
        {
            return PInvoke.UnityIosImagePickerController_IsSourceTypeAvailable(sourceType);
        }

        public bool IsCameraDeviceAvailable(IosImagePickerCameraDevice cameraDevice)
        {
            return PInvoke.UnityIosImagePickerController_IsCameraDeviceAvailable(cameraDevice);
        }

        public string[] AvailableMediaTypesForSourceType(IosImagePickerSourceType sourceType)
        {
            var serializedMediaTypes = PInvoke.UnityIosImagePickerController_AvailableMediaTypesForSourceType(sourceType);
            return serializedMediaTypes != null ? serializedMediaTypes.Split('#') : new string[0];
        }

        public IosImagePickerVideoCaptureMode[] AvailableCaptureModesForCameraDevice(IosImagePickerCameraDevice cameraDevice)
        {
            var serializedCaptureModes = PInvoke.UnityIosImagePickerController_AvailableCaptureModesForCameraDevice(cameraDevice);
            if (serializedCaptureModes == null)
                return new IosImagePickerVideoCaptureMode[0];

            var rawCaptureModes = serializedCaptureModes.Split('#');
            var captureModes = new List<IosImagePickerVideoCaptureMode>();
            for (var i = 0; i < rawCaptureModes.Length; i++)
            {
                int parsedCaptureMode;
                if (int.TryParse(rawCaptureModes[i], out parsedCaptureMode))
                    captureModes.Add((IosImagePickerVideoCaptureMode)parsedCaptureMode);
            }

            return captureModes.ToArray();
        }

        public bool IsFlashAvailableForCameraDevice(IosImagePickerCameraDevice cameraDevice)
        {
            return PInvoke.UnityIosImagePickerController_IsFlashAvailableForCameraDevice(cameraDevice);
        }

        public IosImagePickerSourceType SourceType { get; set; }
        public string[] MediaTypes { get; set; }
        public bool AllowsEditing { get; set; }
        public IosImagePickerVideoQualityType VideoQuality { get; set; }
        public TimeSpan VideoMaximumDuration { get; set; }
        public bool ShowCameraControls { get; set; }
        public IosImagePickerCameraDevice CameraDevice { get; set; }
        public IosImagePickerVideoCaptureMode CameraCaptureMode { get; set; }
        public IosImagePickerCameraFlashMode CameraFlashMode { get; set; }
        
        private static class PInvoke
        {
            [DllImport("__Internal")]
            public static extern bool UnityIosImagePickerController_IsSourceTypeAvailable([MarshalAs(UnmanagedType.SysInt)]IosImagePickerSourceType sourceType);
            
            [DllImport("__Internal")]
            public static extern bool UnityIosImagePickerController_IsCameraDeviceAvailable([MarshalAs(UnmanagedType.SysInt)]IosImagePickerCameraDevice cameraDevice);
            
            [DllImport("__Internal")]
            public static extern string UnityIosImagePickerController_AvailableMediaTypesForSourceType([MarshalAs(UnmanagedType.SysInt)]IosImagePickerSourceType sourceType);
            
            [DllImport("__Internal")]
            public static extern string UnityIosImagePickerController_AvailableCaptureModesForCameraDevice([MarshalAs(UnmanagedType.SysInt)]IosImagePickerCameraDevice cameraDevice);
            
            [DllImport("__Internal")]
            public static extern bool UnityIosImagePickerController_IsFlashAvailableForCameraDevice([MarshalAs(UnmanagedType.SysInt)]IosImagePickerCameraDevice cameraDevice);
        }
    }
}
