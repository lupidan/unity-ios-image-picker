using IosImagePicker.Interfaces;

namespace IosImagePicker.Editor
{
    public class EditorIosError : IIosError
    {
        public int Code { get; }
        public string Domain { get; }
        public string LocalizedDescription { get; }

        public EditorIosError(
            int code,
            string domain,
            string localizedDescription)
        {
            this.Code = code;
            this.Domain = domain;
            this.LocalizedDescription = localizedDescription;
        }
    }
}
