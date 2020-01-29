using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using IosImagePicker;
using UnityEngine;

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

        public IosImagePickerCameraCaptureMode[] AvailableCaptureModesForCameraDevice(IosImagePickerCameraDevice cameraDevice)
        {
            var serializedCaptureModes = PInvoke.UnityIosImagePickerController_AvailableCaptureModesForCameraDevice(cameraDevice);
            if (serializedCaptureModes == null)
                return new IosImagePickerCameraCaptureMode[0];

            var rawCaptureModes = serializedCaptureModes.Split(SerializationSeparator);
            var captureModes = new List<IosImagePickerCameraCaptureMode>();
            for (var i = 0; i < rawCaptureModes.Length; i++)
            {
                int parsedCaptureMode;
                if (int.TryParse(rawCaptureModes[i], out parsedCaptureMode))
                    captureModes.Add((IosImagePickerCameraCaptureMode)parsedCaptureMode);
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
        public IosImagePickerCameraDevice CameraDevice { get; set; }
        public IosImagePickerCameraCaptureMode CameraCaptureMode { get; set; }
        public IosImagePickerCameraFlashMode CameraFlashMode { get; set; }

        public IosImagePicker()
        {
            // Get constants
            this.MediaTypeImage = PInvoke.UnityIosImagePickerController_GetMediaTypeImage();
            this.MediaTypeMovie = PInvoke.UnityIosImagePickerController_GetMediaTypeMovie();
            
            // Keep the same default values as UIKit
            this.SourceType = IosImagePickerSourceType.PhotoLibrary;
            this.MediaTypes = new[] { MediaTypeImage };
            this.AllowsEditing = false;
            this.VideoQuality = IosImagePickerVideoQualityType.Medium;
            this.VideoMaximumDuration = TimeSpan.FromSeconds(600.0);
            this.CameraDevice = IosImagePickerCameraDevice.Rear;
            this.CameraCaptureMode = IosImagePickerCameraCaptureMode.Photo;
            this.CameraFlashMode = IosImagePickerCameraFlashMode.Auto;
        }

        public void Present(Action<IosImagePickerResult> resultCallback)
        {
            PInvoke.UnityIosImagePickerController_SetResultCallback(PInvoke.UnityIosImagePickerControllerResultCallback);

            var requestId = PInvoke.AddCallback(resultCallback);

            var mediaTypes = this.MediaTypes;
            if (mediaTypes == null)
                mediaTypes = AvailableMediaTypesForSourceType(this.SourceType);

            var serializedMediaTypes = string.Join(SerializationSeparator.ToString(), mediaTypes);
            
            PInvoke.UnityIosImagePickerController_Present(
                requestId,
                this.SourceType,
                serializedMediaTypes,
                this.AllowsEditing,
                this.VideoQuality,
                this.VideoMaximumDuration.TotalSeconds,
                this.CameraDevice,
                this.CameraCaptureMode,
                this.CameraFlashMode);
        }

        public void Update()
        {
            PInvoke.ExecuteScheduledCallbacks();
        }
        
        private static class PInvoke
        {
            public struct RawResult
            {
                public bool didCancel;
                public string serializedCropRect;
                public string mediaType;
                public string imageUrl;
                public string originalImageFileUrl;
                public string editedImageFileUrl;
                public string videoFileUrl;
                public string mediaMetadataJson;
            }

            private const int InitialCallbackId = 1;
            private const int MaxCallbackId = int.MaxValue;
            
            private static int _callbackId = InitialCallbackId;
            
            private static readonly Dictionary<int, Action<IosImagePickerResult>> CallbackDictionary = new Dictionary<int, Action<IosImagePickerResult>>();
            private static readonly List<Action> ScheduledCallbacks = new List<Action>();

            public static int AddCallback(Action<IosImagePickerResult> callback)
            {
                var storedCallbackId = _callbackId;
                CallbackDictionary[storedCallbackId] = callback;
                
                _callbackId += 1;
                if (_callbackId >= MaxCallbackId)
                    _callbackId = InitialCallbackId;
                
                return storedCallbackId;
            }
            
            public static void ExecuteScheduledCallbacks()
            {
                for (var i = 0; i < ScheduledCallbacks.Count; i++)
                {
                    var callback = ScheduledCallbacks[i];
                    if (callback != null)
                        callback();
                }

                ScheduledCallbacks.Clear();
            }
            
            public delegate void UnityIosImagePickerControllerResultDelegate(int requestId, string resultJsonPayload);

            [MonoPInvokeCallback(typeof(UnityIosImagePickerControllerResultDelegate))]
            public static void UnityIosImagePickerControllerResultCallback(int requestId, string resultJsonPayload)
            {
                try
                {
                    Action<IosImagePickerResult> callback;
                    if (CallbackDictionary.TryGetValue(requestId, out callback))
                    {
                        CallbackDictionary.Remove(requestId);

                        Debug.Log(resultJsonPayload);
                        
                        if (callback != null)
                            ScheduledCallbacks.Add(() => callback(null));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed handling of callback for request with Id " + requestId + " :: " + e.Message);
                }
            }
            
            [DllImport("__Internal")]
            public static extern void UnityIosImagePickerController_SetResultCallback(UnityIosImagePickerControllerResultDelegate resultCallback);
            
            [DllImport("__Internal")]
            public static extern string UnityIosImagePickerController_GetMediaTypeMovie();
            
            [DllImport("__Internal")]
            public static extern string UnityIosImagePickerController_GetMediaTypeImage();
            
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
                [MarshalAs(UnmanagedType.SysInt)]IosImagePickerCameraDevice cameraDevice,
                [MarshalAs(UnmanagedType.SysInt)]IosImagePickerCameraCaptureMode cameraCaptureMode,
                [MarshalAs(UnmanagedType.SysInt)]IosImagePickerCameraFlashMode cameraFlashMode);
        }
    }
}
