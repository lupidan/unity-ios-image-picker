namespace IosImagePicker.Interfaces
{
    public interface IPayloadDeserializer
    {
        IIosImagePickerResult DeserializeIosImagePickerResult(string payload);
        IIosImagePickerCleanupResult DeserializeIosImageCleanupResult(string payload);
    }
}
