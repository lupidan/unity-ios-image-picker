using System;

namespace IosImagePicker.Enums
{
    /// <summary>
    /// UIPopoverArrowDirection
    /// Constants for specifying the direction of the popover arrow.
    /// </summary>
    [Flags]
    public enum IosImagePickerPopoverArrowDirection : uint
    {
        /// <summary>
        /// An arrow that points upward.
        /// </summary>
        Up = 1U << 0,
        
        /// <summary>
        /// An arrow that points downward.
        /// </summary>
        Down = 1U << 1,
        
        /// <summary>
        /// An arrow that points toward the left.
        /// </summary>
        Left = 1U << 2,
        
        /// <summary>
        /// An arrow that points toward the right.
        /// </summary>
        Right = 1U << 3,
        
        /// <summary>
        /// An arrow that points in any direction.
        /// </summary>
        Any = Up | Down | Left | Right,
        
        /// <summary>
        /// The status of the arrow is currently unknown.
        /// </summary>
        Unknown = uint.MaxValue, 
    }
}
