using IosImagePicker.Interfaces;
using UnityEngine;

namespace IosImagePicker.IOS.NativeMessages
{
    public static class NativePayloadDeserializer
    {
        public static IIosImagePickerResult DeserializeIosImagePickerResult(string payload)
        {
            return JsonUtility.FromJson<NativeIosImagePickerResult>(payload);
        }

        public static IIosImagePickerCleanupResult DeserializeIosImageCleanupResult(string payload)
        {
            return JsonUtility.FromJson<NativeIosImagePickerCleanupResult>(payload);
        }
    }
}
