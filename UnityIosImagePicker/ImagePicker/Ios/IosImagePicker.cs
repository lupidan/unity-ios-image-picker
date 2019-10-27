﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ImagePicker.Ios
{
    public class IosImagePicker : IIosImagePicker
    {
        private const char SerializationSeparator = '#';
        
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
            return serializedMediaTypes != null ? serializedMediaTypes.Split(SerializationSeparator) : new string[0];
        }

        public IosImagePickerVideoCaptureMode[] AvailableCaptureModesForCameraDevice(IosImagePickerCameraDevice cameraDevice)
        {
            var serializedCaptureModes = PInvoke.UnityIosImagePickerController_AvailableCaptureModesForCameraDevice(cameraDevice);
            if (serializedCaptureModes == null)
                return new IosImagePickerVideoCaptureMode[0];

            var rawCaptureModes = serializedCaptureModes.Split(SerializationSeparator);
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

        public string MediaTypeImage { get; private set; }
        public string MediaTypeMovie { get; private set; }
        public IosImagePickerSourceType SourceType { get; set; }
        public string[] MediaTypes { get; set; }
        public bool AllowsEditing { get; set; }
        public IosImagePickerVideoQualityType VideoQuality { get; set; }
        public TimeSpan VideoMaximumDuration { get; set; }
        public bool ShowCameraControls { get; set; }
        public IosImagePickerCameraDevice CameraDevice { get; set; }
        public IosImagePickerVideoCaptureMode CameraCaptureMode { get; set; }
        public IosImagePickerCameraFlashMode CameraFlashMode { get; set; }

        public IosImagePicker()
        {
            // Get constants
            this.MediaTypeImage = PInvoke.UnityIosImagePickerController_GetMediaTypeImage();
            this.MediaTypeMovie = PInvoke.UnityIosImagePickerController_GetMediaTypeVideo();
            
            // Keep the same default values as UIKit
            this.SourceType = IosImagePickerSourceType.PhotoLibrary;
            this.MediaTypes = new[] { MediaTypeImage };
            this.AllowsEditing = false;
            this.VideoQuality = IosImagePickerVideoQualityType.Medium;
            this.VideoMaximumDuration = TimeSpan.FromSeconds(600.0);
            this.ShowCameraControls = true;
            this.CameraDevice = IosImagePickerCameraDevice.Rear;
            this.CameraCaptureMode = IosImagePickerVideoCaptureMode.Photo;
            this.CameraFlashMode = IosImagePickerCameraFlashMode.Auto;
        }

        public void Present()
        {
            var mediaTypes = this.MediaTypes;
            if (mediaTypes == null)
                mediaTypes = AvailableMediaTypesForSourceType(this.SourceType);

            var serializedMediaTypes = string.Join(SerializationSeparator.ToString(), mediaTypes);
            
            PInvoke.UnityIosImagePickerController_Present(
                1,
                this.SourceType,
                serializedMediaTypes,
                this.AllowsEditing,
                this.VideoQuality,
                this.VideoMaximumDuration.TotalSeconds,
                this.ShowCameraControls,
                this.CameraDevice,
                this.CameraCaptureMode,
                this.CameraFlashMode);
        }
        
        private static class PInvoke
        {
            [DllImport("__Internal")]
            public static extern string UnityIosImagePickerController_GetMediaTypeImage();
            
            [DllImport("__Internal")]
            public static extern string UnityIosImagePickerController_GetMediaTypeVideo();
            
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
            
            [DllImport("__Internal")]
            public static extern void UnityIosImagePickerController_Present(
                int requestId,
                [MarshalAs(UnmanagedType.SysInt)]IosImagePickerSourceType sourceType,
                string serializedMediaTypes,
                bool allowsEditing,
                [MarshalAs(UnmanagedType.SysInt)]IosImagePickerVideoQualityType videoQuality,
                double videoMaximumDurationInSeconds,
                bool showCameraControls,
                [MarshalAs(UnmanagedType.SysInt)]IosImagePickerCameraDevice cameraDevice,
                [MarshalAs(UnmanagedType.SysInt)]IosImagePickerVideoCaptureMode cameraCaptureMode,
                [MarshalAs(UnmanagedType.SysInt)]IosImagePickerCameraFlashMode cameraFlashMode);
        }
    }
}