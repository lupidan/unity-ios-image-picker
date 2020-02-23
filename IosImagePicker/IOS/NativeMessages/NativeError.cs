using IosImagePicker.Interfaces;
using System;
using UnityEngine;

namespace IosImagePicker.IOS.NativeMessages
{
    [Serializable]
    public class NativeError : IIosError, ISerializationCallbackReceiver
    {
        public int _code;
        public string _domain;
        public string _localizedDescription;
        
        public int Code { get { return this._code; } }
        public string Domain { get { return this._domain; } }
        public string LocalizedDescription { get { return this._localizedDescription; } }
        
        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            NativeSerializationTools.FixSerializationForString(ref this._domain);
            NativeSerializationTools.FixSerializationForString(ref this._localizedDescription);
        }
    }
}
