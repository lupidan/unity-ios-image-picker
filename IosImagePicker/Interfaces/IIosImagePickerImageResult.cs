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
        /// Original image file path.
        /// On iOS, it contains the file path of the image returned by UIImagePickerControllerOriginalImage that was saved in the temp folder.
        /// </summary>
        string OriginalImageFilePath { get; }
        
        /// <summary>
        /// Error if there was some problem while retrieving the original image file.
        /// </summary>
        IIosError OriginalImageError { get; }
        
        /// <summary>
        /// Edited image file path if image picker allowed editing. 
        /// On iOS, it contains the file path of the image returned by UIImagePickerControllerEditedImage that was saved in the temp folder.
        /// </summary>
        string EditedImageFilePath { get; }
        
        /// <summary>
        /// Error if there was some problem while retrieving the edited image file.
        /// </summary>
        IIosError EditedImageError { get; }
        
        /// <summary>
        /// Image file path as returned from iOS 11 Onwards.
        /// On iOS, it contains the file path returned by UIImagePickerControllerImageURL, that was copied in the temp folder.
        /// </summary>
        string ImageFilePath { get; }
        
        /// <summary>
        /// Error if there was some problem while retrieving the image file.
        /// </summary>
        IIosError ImageError { get; }
    }
}
