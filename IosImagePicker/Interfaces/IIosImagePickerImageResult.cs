using UnityEngine;

namespace IosImagePicker.Interfaces
{
    public interface IIosImagePickerImageResult
    {
        Rect CropRect { get; }    
        string FileUrl { get; }
        string OriginalFileUrl { get; }
    }
}
