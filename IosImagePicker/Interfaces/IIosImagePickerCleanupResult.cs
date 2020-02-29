namespace IosImagePicker.Interfaces
{
    public interface IIosImagePickerCleanupResult
    {
        /// <summary>
        /// List of deletion entries as the result of the cleanup.
        /// </summary>
        IIosImagePickerDeletionEntry[] DeletionEntries { get; }
    }
}
