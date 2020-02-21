namespace IosImagePicker.Interfaces
{
    public interface IPayloadDeserializer
    {
        IIosImagePickerResult DeserializeIosImagePickerResult(string payload);
    }
}
