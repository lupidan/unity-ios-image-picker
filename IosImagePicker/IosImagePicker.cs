﻿#if !UNITY_EDITOR && UNITY_IOS
#define IOS_IMAGE_PICKER_NATIVE_IMPLEMENTATION_AVAILABLE
#endif

using System;
using IosImagePicker.Enums;
using IosImagePicker.Interfaces;

namespace IosImagePicker
{
    public class IosImagePicker : IIosImagePicker
    {
#if IOS_IMAGE_PICKER_NATIVE_IMPLEMENTATION_AVAILABLE
        private const char SerializationSeparator = '#';
        
        private readonly IPayloadDeserializer _payloadDeserializer;
#endif
        
        public bool IsSourceTypeAvailable(IosImagePickerSourceType sourceType)
        {
#if IOS_IMAGE_PICKER_NATIVE_IMPLEMENTATION_AVAILABLE
            return PInvoke.UnityIosImagePickerController_IsSourceTypeAvailable(sourceType);
#else
            return false;
#endif
        }

        public bool IsCameraDeviceAvailable(IosImagePickerCameraDevice cameraDevice)
        {
#if IOS_IMAGE_PICKER_NATIVE_IMPLEMENTATION_AVAILABLE
            return PInvoke.UnityIosImagePickerController_IsCameraDeviceAvailable(cameraDevice);
#else
            return false;
#endif
        }

        public string[] AvailableMediaTypesForSourceType(IosImagePickerSourceType sourceType)
        {
#if IOS_IMAGE_PICKER_NATIVE_IMPLEMENTATION_AVAILABLE
            var serializedMediaTypes = PInvoke.UnityIosImagePickerController_AvailableMediaTypesForSourceType(sourceType);
            return serializedMediaTypes != null ? serializedMediaTypes.Split(SerializationSeparator) : new string[0];
#else
            return new string[0];
#endif
        }

        public IosImagePickerCameraCaptureMode[] AvailableCaptureModesForCameraDevice(IosImagePickerCameraDevice cameraDevice)
        {
#if IOS_IMAGE_PICKER_NATIVE_IMPLEMENTATION_AVAILABLE
            var serializedCaptureModes = PInvoke.UnityIosImagePickerController_AvailableCaptureModesForCameraDevice(cameraDevice);
            if (serializedCaptureModes == null)
                return new IosImagePickerCameraCaptureMode[0];

            var rawCaptureModes = serializedCaptureModes.Split(SerializationSeparator);
            var captureModes = new System.Collections.Generic.List<IosImagePickerCameraCaptureMode>();
            for (var i = 0; i < rawCaptureModes.Length; i++)
            {
                int parsedCaptureMode;
                if (int.TryParse(rawCaptureModes[i], out parsedCaptureMode))
                    captureModes.Add((IosImagePickerCameraCaptureMode)parsedCaptureMode);
            }

            return captureModes.ToArray();
#else
            return new IosImagePickerCameraCaptureMode[0];
#endif
        }

        public bool IsFlashAvailableForCameraDevice(IosImagePickerCameraDevice cameraDevice)
        {
#if IOS_IMAGE_PICKER_NATIVE_IMPLEMENTATION_AVAILABLE
            return PInvoke.UnityIosImagePickerController_IsFlashAvailableForCameraDevice(cameraDevice);
#else
            return false;
#endif
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

        public IosImagePicker(IPayloadDeserializer payloadDeserializer)
        {
#if IOS_IMAGE_PICKER_NATIVE_IMPLEMENTATION_AVAILABLE
            this._payloadDeserializer = payloadDeserializer;
            this.MediaTypeImage = PInvoke.UnityIosImagePickerController_GetMediaTypeImage();
            this.MediaTypeMovie = PInvoke.UnityIosImagePickerController_GetMediaTypeMovie();
#else
            this.MediaTypeImage = "public.image";
            this.MediaTypeMovie = "public.movie";
#endif
            this.SourceType = IosImagePickerSourceType.PhotoLibrary;
            this.MediaTypes = new[] { MediaTypeImage };
            this.AllowsEditing = false;
            this.VideoQuality = IosImagePickerVideoQualityType.Medium;
            this.VideoMaximumDuration = TimeSpan.FromSeconds(600.0);
            this.CameraDevice = IosImagePickerCameraDevice.Rear;
            this.CameraCaptureMode = IosImagePickerCameraCaptureMode.Photo;
            this.CameraFlashMode = IosImagePickerCameraFlashMode.Auto;
        }

        public void Present(Action<IIosImagePickerResult> resultCallback)
        {
#if IOS_IMAGE_PICKER_NATIVE_IMPLEMENTATION_AVAILABLE
            var requestId = CallbackHandler.AddCallback(payload =>
            {
                var result = this._payloadDeserializer.DeserializeIosImagePickerResult(payload);
                resultCallback.Invoke(result);
            });

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
#else
            throw new Exception("Ios Image Picker not supported in this platform");
#endif
        }

        public void Update()
        {
#if IOS_IMAGE_PICKER_NATIVE_IMPLEMENTATION_AVAILABLE
            CallbackHandler.ExecutePendingCallbacks();
#endif
        }

#if IOS_IMAGE_PICKER_NATIVE_IMPLEMENTATION_AVAILABLE
        private static class CallbackHandler
        {
            private const uint InitialCallbackId = 1U;
            private const uint MaxCallbackId = uint.MaxValue;
            
            private static readonly object SyncLock = new object();
            
            private static uint _callbackId = InitialCallbackId;
            private static bool _initialized = false;
            
            private static readonly System.Collections.Generic.Dictionary<uint, Action<string>> CallbackDictionary = new System.Collections.Generic.Dictionary<uint, Action<string>>();
            private static readonly System.Collections.Generic.List<Action> ScheduledCallbacks = new System.Collections.Generic.List<Action>();
            
            public static uint AddCallback(Action<string> callback)
            {
                if (!_initialized)
                {
                    PInvoke.UnityIosImagePickerController_SetResultCallback(PInvoke.UnityIosImagePickerControllerResultCallback);
                    _initialized = true;
                }
                
                if (callback == null)
                {
                    throw new Exception("Can't add a null callback.");
                }
                
                var usedCallbackId = 0U;
                lock (SyncLock)
                {
                    usedCallbackId = _callbackId;
                    CallbackDictionary[usedCallbackId] = callback;
                
                    _callbackId += 1;
                    if (_callbackId >= MaxCallbackId)
                        _callbackId = InitialCallbackId;
                }
                
                return usedCallbackId;
            }
            
            public static void ScheduleCallback(uint requestId, string payload)
            {
                lock (SyncLock)
                {
                    var callback = default(Action<string>);
                    if (CallbackDictionary.TryGetValue(requestId, out callback))
                    {
                        ScheduledCallbacks.Add(() => callback.Invoke(payload));
                        CallbackDictionary.Remove(requestId);
                    }
                }
            }
            
            public static void ExecutePendingCallbacks()
            {
                lock (SyncLock)
                {
                    while (ScheduledCallbacks.Count > 0)
                    {
                        var action = ScheduledCallbacks[0];
                        ScheduledCallbacks.RemoveAt(0);
                        action.Invoke();
                    }
                }
            }
        }

        private static class PInvoke
        {
            public delegate void UnityIosImagePickerControllerResultDelegate(uint requestId, string payload);

            [AOT.MonoPInvokeCallback(typeof(UnityIosImagePickerControllerResultDelegate))]
            public static void UnityIosImagePickerControllerResultCallback(uint requestId, string payload)
            {
                try
                {
                    CallbackHandler.ScheduleCallback(requestId, payload);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed handling of callback for request with Id " + requestId + " :: " + e.Message);
                }
            }
            
            [System.Runtime.InteropServices.DllImport("__Internal")]
            public static extern void UnityIosImagePickerController_SetResultCallback(UnityIosImagePickerControllerResultDelegate resultCallback);
            
            [System.Runtime.InteropServices.DllImport("__Internal")]
            public static extern string UnityIosImagePickerController_GetMediaTypeMovie();
            
            [System.Runtime.InteropServices.DllImport("__Internal")]
            public static extern string UnityIosImagePickerController_GetMediaTypeImage();
            
            [System.Runtime.InteropServices.DllImport("__Internal")]
            public static extern bool UnityIosImagePickerController_IsSourceTypeAvailable(
                [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.SysInt)]IosImagePickerSourceType sourceType);
            
            [System.Runtime.InteropServices.DllImport("__Internal")]
            public static extern bool UnityIosImagePickerController_IsCameraDeviceAvailable(
                [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.SysInt)]IosImagePickerCameraDevice cameraDevice);
            
            [System.Runtime.InteropServices.DllImport("__Internal")]
            public static extern string UnityIosImagePickerController_AvailableMediaTypesForSourceType(
                [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.SysInt)]IosImagePickerSourceType sourceType);
            
            [System.Runtime.InteropServices.DllImport("__Internal")]
            public static extern string UnityIosImagePickerController_AvailableCaptureModesForCameraDevice(
                [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.SysInt)]IosImagePickerCameraDevice cameraDevice);
            
            [System.Runtime.InteropServices.DllImport("__Internal")]
            public static extern bool UnityIosImagePickerController_IsFlashAvailableForCameraDevice(
                [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.SysInt)]IosImagePickerCameraDevice cameraDevice);
            
            [System.Runtime.InteropServices.DllImport("__Internal")]
            public static extern void UnityIosImagePickerController_Present(
                uint requestId,
                [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.SysInt)]IosImagePickerSourceType sourceType,
                string serializedMediaTypes,
                bool allowsEditing,
                [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.SysInt)]IosImagePickerVideoQualityType videoQuality,
                double videoMaximumDurationInSeconds,
                [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.SysInt)]IosImagePickerCameraDevice cameraDevice,
                [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.SysInt)]IosImagePickerCameraCaptureMode cameraCaptureMode,
                [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.SysInt)]IosImagePickerCameraFlashMode cameraFlashMode);
        }
#endif
    }
}
