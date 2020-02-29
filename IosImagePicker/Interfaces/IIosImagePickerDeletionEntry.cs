namespace IosImagePicker.Interfaces
{
    public interface IIosImagePickerDeletionEntry
    {
        /// <summary>
        /// Whether the entry corresponds to a directory or not.
        /// </summary>
        bool IsDirectory { get; }
        
        /// <summary>
        /// When on preview mode, this indicates if the file would have been deleted or not.
        /// </summary>
        bool WouldDelete { get; }
        
        /// <summary>
        /// Whether the file was deleted or not.
        /// </summary>
        bool Deleted { get; }
        
        /// <summary>
        /// Path of the file.
        /// </summary>
        string Path { get; }
        
        /// <summary>
        /// Size of the file in bytes.
        /// </summary>
        ulong FileSize { get; }
    }
}
