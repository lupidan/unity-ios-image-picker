using IosImagePicker.Interfaces;
using System;
using UnityEngine;

namespace IosImagePicker.IOS.NativeMessages
{
    [Serializable]
    public class NativeIosImagePickerDeletionEntry : IIosImagePickerDeletionEntry, ISerializationCallbackReceiver
    {
        public bool _isDirectory;
        public bool _wouldDelete;
        public bool _deleted;
        public string _path;
        public ulong _fileSize;
        
        public bool IsDirectory { get { return this._isDirectory; } }
        public bool WouldDelete { get { return this._wouldDelete; } }
        public bool Deleted { get { return this._deleted; } }
        public string Path { get { return this._path; } }
        public ulong FileSize { get { return this._fileSize; } }
        
        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            NativeSerializationTools.FixSerializationForString(ref this._path);
        }
    }
}
