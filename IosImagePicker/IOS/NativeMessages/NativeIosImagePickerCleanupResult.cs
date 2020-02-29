using IosImagePicker.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IosImagePicker.IOS.NativeMessages
{
    [Serializable]
    public class NativeIosImagePickerCleanupResult : IIosImagePickerCleanupResult, ISerializationCallbackReceiver
    {
        public NativeIosImagePickerDeletionEntry[] _deletionEntries;

        public IIosImagePickerDeletionEntry[] DeletionEntries { get; private set; }

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            var castedDeletionEntries = new List<IIosImagePickerDeletionEntry>();
            for (var i = 0; i < this._deletionEntries.Length; i++)
            {
                castedDeletionEntries.Add(this._deletionEntries[i]);
            }

            this.DeletionEntries = castedDeletionEntries.ToArray();
        }
    }
}
