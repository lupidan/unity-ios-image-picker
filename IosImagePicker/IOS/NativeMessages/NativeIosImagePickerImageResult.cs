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
        public string _editedImageFileUrl;
        public string _imageFileUrl;
        
        public Rect CropRect { get; private set; }
        public string OriginalImageFileUrl { get { return this._originalImageFileUrl; } }
        public string EditedImageFileUrl { get { return this._editedImageFileUrl; } }
        public string ImageFileUrl { get { return this._imageFileUrl; } }
        
        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            NativeSerializationTools.FixSerializationForString(ref this._originalImageFileUrl);
            NativeSerializationTools.FixSerializationForString(ref this._editedImageFileUrl);
            NativeSerializationTools.FixSerializationForString(ref this._imageFileUrl);

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
