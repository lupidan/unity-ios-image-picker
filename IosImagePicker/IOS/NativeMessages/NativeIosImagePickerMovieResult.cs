using IosImagePicker.Interfaces;
using System;
using UnityEngine;

namespace IosImagePicker.IOS.NativeMessages
{
    [Serializable]
    public class NativeIosImagePickerMovieResult : IIosImagePickerMovieResult, ISerializationCallbackReceiver
    {
        public string _movieFileUrl;

        public string MovieFileUrl { get { return this._movieFileUrl; } }

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            NativeSerializationTools.FixSerializationForString(ref this._movieFileUrl);
        }
    }
}
