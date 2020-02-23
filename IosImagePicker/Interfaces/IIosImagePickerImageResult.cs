using UnityEngine;

namespace IosImagePicker.Interfaces
{
    public interface IIosImagePickerImageResult
    {
        /// <summary>
        /// The crop rect used in the original image to get the edited image.
        /// Or CGRect.Zero if no edit happened.
        /// </summary>
        Rect CropRect { get; }
        
        /// <summary>
        /// Original image file URL.
        /// Contains the file URL of the image returned by UIImagePickerControllerOriginalImage that was saved in the temp folder.
        /// </summary>
        string OriginalImageFileUrl { get; }
        
        /// <summary>
        /// Error if there was some problem while retrieving the original image file.
        /// </summary>
        IIosError OriginalImageError { get; }
        
        /// <summary>
        /// Edited image file URL if image picker allowed editing. 
        /// Contains the file URL of the image returned by UIImagePickerControllerEditedImage that was saved in the temp folder.
        /// </summary>
        string EditedImageFileUrl { get; }
        
        /// <summary>
        /// Error if there was some problem while retrieving the edited image file.
        /// </summary>
        IIosError EditedImageError { get; }
        
        /// <summary>
        /// Image file URL as returned from iOS 11 Onwards.
        /// Contains the file url returned by UIImagePickerControllerImageURL.
        /// </summary>
        string ImageFileUrl { get; }
        
        /// <summary>
        /// Error if there was some problem while retrieving the image file.
        /// </summary>
        IIosError ImageError { get; }
    }
}
