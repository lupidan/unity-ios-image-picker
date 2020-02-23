using IosImagePicker.Interfaces;
using System;
using UnityEngine;

namespace IosImagePicker.IOS.NativeMessages
{
    [Serializable]
    public class NativeIosImagePickerResult : IIosImagePickerResult, ISerializationCallbackReceiver
    {
        public bool _didCancel;
        public string _mediaType;
        public bool _hasImage;
        public NativeIosImagePickerImageResult _image;
        public bool _hasMovie;
        public NativeIosImagePickerMovieResult _movie;
        public string _mediaMetadataJson;

        public bool DidCancel { get { return this._didCancel; } }
        public string MediaType { get { return this._mediaType; } }
        public IIosImagePickerImageResult Image { get { return this._image; } }
        public IIosImagePickerMovieResult Movie { get { return this._movie; } }
        public string MediaMetadataJson { get { return this._mediaMetadataJson; } }
        
        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            NativeSerializationTools.FixSerializationForString(ref this._mediaType);
            NativeSerializationTools.FixSerializationForString(ref this._mediaMetadataJson);
            NativeSerializationTools.FixSerializationForObject(ref this._image, this._hasImage);
            NativeSerializationTools.FixSerializationForObject(ref this._movie, this._hasMovie);
        }
    }
}
