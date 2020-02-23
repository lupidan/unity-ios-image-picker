using IosImagePicker.Interfaces;
using System;
using UnityEngine;

namespace IosImagePicker.IOS.NativeMessages
{
    [Serializable]
    public class NativeIosImagePickerMovieResult : IIosImagePickerMovieResult, ISerializationCallbackReceiver
    {
        public string _movieFileUrl;
        public bool _hasMovieFileUrlError;
        public NativeError _movieFileUrlError;

        public string MovieFileUrl { get { return this._movieFileUrl; } }
        public IIosError MovieFileError { get { return this._movieFileUrlError; }  }

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            NativeSerializationTools.FixSerializationForString(ref this._movieFileUrl);
            NativeSerializationTools.FixSerializationForObject(ref this._movieFileUrlError, this._hasMovieFileUrlError);
        }
    }
}
