using IosImagePicker.Interfaces;
using UnityEngine;

namespace IosImagePicker.IOS.NativeMessages
{
    public class PayloadDeserializer : IPayloadDeserializer
    {
        public IIosImagePickerResult DeserializeIosImagePickerResult(string payload)
        {
            return JsonUtility.FromJson<NativeIosImagePickerResult>(payload);
        }
    }
}
