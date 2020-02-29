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
            CropRect = cropRect;
            OriginalImageFilePath = originalImageFilePath;
            OriginalImageError = originalImageError;
            EditedImageFilePath = editedImageFilePath;
            EditedImageError = editedImageError;
            ImageFilePath = imageFilePath;
            ImageError = imageError;
        }
    }
}
