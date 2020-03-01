using IosImagePicker.Enums;
using UnityEngine;

namespace IosImagePicker
{
    public struct IosImagePickerPresentArgs
    {
        /// <summary>
        /// Use this property to specify the anchor location for the popover on iPad.
        /// The rect it's represented as values from 0 to 1 in each axes:
        /// (0,0) being the BOTTOM LEFT corner
        /// (1,1) being the TOP RIGHT corner
        /// </summary>
        public readonly Rect IpadNormalizedPopoverSourceRect;
        
        /// <summary>
        /// Set this property to the arrow directions that you allow for your popover on iPad.
        /// </summary>
        public readonly IosImagePickerPopoverArrowDirection IpadPopoverPermittedArrowDirections;
        
        /// <summary>
        /// Setting this property to true allows the popover to overlap the IpadPopoverSourceRect when space is constrained on iPad.
        /// </summary>
        public readonly bool IpadPopoverCanOverlapSourceRect;

        public IosImagePickerPresentArgs(
            Rect ipadNormalizedPopoverSourceRect = default(Rect),
            IosImagePickerPopoverArrowDirection ipadPopoverPermittedArrowDirections = IosImagePickerPopoverArrowDirection.Any,
            bool ipadPopoverCanOverlapSourceRect = false)
        {
            this.IpadNormalizedPopoverSourceRect = ipadNormalizedPopoverSourceRect;
            this.IpadPopoverPermittedArrowDirections = ipadPopoverPermittedArrowDirections;
            this.IpadPopoverCanOverlapSourceRect = ipadPopoverCanOverlapSourceRect;
        }
    }
}
