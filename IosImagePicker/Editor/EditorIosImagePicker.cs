using Boo.Lang;
using IosImagePicker.Enums;
using IosImagePicker.Interfaces;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace IosImagePicker.Editor
{
    public class EditorIosImagePicker : IIosImagePicker
    {
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

        private const string ImageFilter = "png,jpg,jpeg";
        private const string MovieFilter = "mp4,mov,avi";

        private readonly List<Action> _scheduledCallbacks = new List<Action>();
        private readonly object _syncLock = new object();
        
        public void Present(Action<IIosImagePickerResult> resultCallback)
        {
            var filters = new List<string>();
            if (Array.IndexOf(this.MediaTypes, this.MediaTypeImage) != -1)
            {
                filters.Add("Image files");
                filters.Add(ImageFilter);
            }
            
            if (Array.IndexOf(this.MediaTypes, this.MediaTypeMovie) != -1)
            {
                filters.Add("Movie files");
                filters.Add(MovieFilter);
            }
            
            var result = default(EditorIosImagePickerResult);
            var filePath = EditorUtility.OpenFilePanelWithFilters("Open file", "", filters.ToArray());
            if (!string.IsNullOrEmpty(filePath))
            {
                var mediaType = default(string);
                var extension = Path.GetExtension(filePath).TrimStart('.').ToLower();
                if (ImageFilter.Contains(extension))
                {
                    mediaType = this.MediaTypeImage;
                }
                else if (MovieFilter.Contains(extension))
                {
                    mediaType = this.MediaTypeMovie;
                }
                
                var image = default(EditorIosImagePickerImageResult);
                var movie = default(EditorIosImagePickerMovieResult);
                if (mediaType == this.MediaTypeImage)
                {
                    image = new EditorIosImagePickerImageResult(
                        Rect.zero,
                        filePath,
                        null,
                        null,
                        null,
                        filePath,
                        null);
                }
                else if (mediaType == this.MediaTypeMovie)
                {
                    movie = new EditorIosImagePickerMovieResult(
                        filePath,
                        null);
                }
                
                result = new EditorIosImagePickerResult(
                    false,
                    mediaType,
                    image,
                    movie,
                    null);
            }
            else
            {
                result = new EditorIosImagePickerResult(
                    true,
                    null,
                    null,
                    null,
                    null);
            }
            
            lock (this._syncLock)
            {
                this._scheduledCallbacks.Add(() => resultCallback.Invoke(result));
            }
        }

        public void Update()
        {
            lock (this._syncLock)
            {
                while (this._scheduledCallbacks.Count > 0)
                {
                    var action = this._scheduledCallbacks[0];
                    this._scheduledCallbacks.RemoveAt(0);
                    action.Invoke();
                }
            }
        }

        public IIosImagePickerCleanupResult CleanupPluginFolder(bool preview)
        {
            return null;
        }
    }
}
