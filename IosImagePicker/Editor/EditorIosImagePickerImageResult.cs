using IosImagePicker.Interfaces;
using UnityEngine;

namespace IosImagePicker.Editor
{
    public class EditorIosImagePickerImageResult : IIosImagePickerImageResult
    {
        public Rect CropRect { get; }
        public string OriginalImageFilePath { get; }
        public IIosError OriginalImageError { get; }
        public string EditedImageFilePath { get; }
        public IIosError EditedImageError { get; }
        public string ImageFilePath { get; }
        public IIosError ImageError { get; }

        public EditorIosImagePickerImageResult(
            Rect cropRect,
            string originalImageFilePath,
            EditorIosError originalImageError,
            string editedImageFilePath,
            EditorIosError editedImageError,
            string imageFilePath,
            EditorIosError imageError)
        {
            this.CropRect = cropRect;
            this.OriginalImageFilePath = originalImageFilePath;
            this.OriginalImageError = originalImageError;
            this.EditedImageFilePath = editedImageFilePath;
            this.EditedImageError = editedImageError;
            this.ImageFilePath = imageFilePath;
            this.ImageError = imageError;
        }
    }
}
