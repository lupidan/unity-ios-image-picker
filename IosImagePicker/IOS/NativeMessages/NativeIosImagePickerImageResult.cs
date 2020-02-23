using IosImagePicker.Interfaces;
using System;
using UnityEngine;

namespace IosImagePicker.IOS.NativeMessages
{
    [Serializable]
    public class NativeIosImagePickerImageResult : IIosImagePickerImageResult, ISerializationCallbackReceiver
    {
        public bool _hasCropRect;
        public NativeCoreGraphicsRect _cropRect;
        public string _originalImageFileUrl;
        public bool _hasOriginalImageError;
        public NativeError _originalImageError;
        public string _editedImageFileUrl;
        public bool _hasEditedImageError;
        public NativeError _editedImageError;
        public string _imageFileUrl;
        public bool _hasImageFileError;
        public NativeError _imageFileError;
        
        public Rect CropRect { get; private set; }
        public string OriginalImageFileUrl { get { return this._originalImageFileUrl; } }
        public IIosError OriginalImageError { get { return this._originalImageError; } }
        public string EditedImageFileUrl { get { return this._editedImageFileUrl; } }
        public IIosError EditedImageError { get { return this._editedImageError; } }
        public string ImageFileUrl { get { return this._imageFileUrl; } }
        public IIosError ImageError { get { return this._imageFileError; } }

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            NativeSerializationTools.FixSerializationForString(ref this._originalImageFileUrl);
            NativeSerializationTools.FixSerializationForString(ref this._editedImageFileUrl);
            NativeSerializationTools.FixSerializationForString(ref this._imageFileUrl);
            NativeSerializationTools.FixSerializationForObject(ref this._originalImageError, this._hasOriginalImageError);
            NativeSerializationTools.FixSerializationForObject(ref this._editedImageError, this._hasEditedImageError);
            NativeSerializationTools.FixSerializationForObject(ref this._imageFileError, this._hasImageFileError);

            if (this._hasCropRect)
            {
                this.CropRect = new Rect(
                    this._cropRect._originX,
                    this._cropRect._originY,
                    this._cropRect._sizeWidth,
                    this._cropRect._sizeHeight);
            }
            else
            {
                this.CropRect = Rect.zero;
            }
        }
    }
}
