using IosImagePicker.Interfaces;
using System;
using UnityEngine;

namespace IosImagePicker.IOS.NativeMessages
{
    [Serializable]
    public class NativeIosImagePickerMovieResult : IIosImagePickerMovieResult, ISerializationCallbackReceiver
    {
        public string _movieFilePath;
        public bool _hasMovieFileError;
        public NativeError _movieFileError;

        public string MovieFilePath { get { return this._movieFilePath; } }
        public IIosError MovieFileError { get { return this._movieFileError; }  }

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            NativeSerializationTools.FixSerializationForString(ref this._movieFilePath);
            NativeSerializationTools.FixSerializationForObject(ref this._movieFileError, this._hasMovieFileError);
        }
    }
}
