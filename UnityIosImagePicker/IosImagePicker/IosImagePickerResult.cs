using UnityEngine;

namespace IosImagePicker
{
    public class IosImagePickerResult
    {
        public readonly bool DidCancel;
        public readonly Rect CropRect;
        public readonly string MediaType;

        public readonly string ImageFileUrl;
        public readonly string OriginalImageFileUrl;
        public readonly string EditedImageFileUrl;

        public readonly string VideoFileUrl;

        public readonly string MediaMetadataJson;

        public IosImagePickerResult(
            bool didCancel,
            Rect cropRect, 
            string mediaType, 
            string imageFileUrl, 
            string originalImageFileUrl, 
            string editedImageFileUrl, 
            string videoFileUrl, 
            string mediaMetadataJson)
        {
            DidCancel = didCancel;
            CropRect = cropRect;
            MediaType = mediaType;
            ImageFileUrl = imageFileUrl;
            OriginalImageFileUrl = originalImageFileUrl;
            EditedImageFileUrl = editedImageFileUrl;
            VideoFileUrl = videoFileUrl;
            MediaMetadataJson = mediaMetadataJson;
        }
    }
}