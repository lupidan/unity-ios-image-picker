namespace IosImagePicker.Interfaces
{
    public interface IPayloadDeserializer
    {
        IIosImagePickerResult DeserializeIosImagePickerResult(string payload);
        IIosImagePickerDeletionEntry[] DeserializeIosImagePickerEntries(string payload);
    }
}
