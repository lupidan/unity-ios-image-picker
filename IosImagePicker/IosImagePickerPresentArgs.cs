using IosImagePicker.Enums;
using UnityEngine;

namespace IosImagePicker
{
    public struct IosImagePickerPresentArgs
    {
        /// <summary>
        /// Use this property to specify the anchor location for the popover on iPad.
        /// </summary>
        public readonly Rect IpadPopoverSourceRect;
        
        /// <summary>
        /// Set this property to the arrow directions that you allow for your popover on iPad.
        /// </summary>
        public readonly IosImagePickerPopoverArrowDirection IpadPopoverPermittedArrowDirections;
        
        /// <summary>
        /// Setting this property to true allows the popover to overlap the IpadPopoverSourceRect when space is constrained on iPad.
        /// </summary>
        public readonly bool IpadPopoverCanOverlapSourceRect;

        public IosImagePickerPresentArgs(
            Rect ipadPopoverSourceRect = default(Rect),
            IosImagePickerPopoverArrowDirection ipadPopoverPermittedArrowDirections = IosImagePickerPopoverArrowDirection.Any,
            bool ipadPopoverCanOverlapSourceRect = false)
        {
            this.IpadPopoverSourceRect = ipadPopoverSourceRect;
            this.IpadPopoverPermittedArrowDirections = ipadPopoverPermittedArrowDirections;
            this.IpadPopoverCanOverlapSourceRect = ipadPopoverCanOverlapSourceRect;
        }
    }
}
