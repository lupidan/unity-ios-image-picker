namespace IosImagePicker.Interfaces
{
    public interface IIosError
    {
        int Code { get; }
        string Domain { get; }
        string LocalizedDescription { get; }
    }
}
