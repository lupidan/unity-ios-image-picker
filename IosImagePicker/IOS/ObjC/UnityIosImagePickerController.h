#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface UnityIosImagePickerController : NSObject

+ (instancetype) defaultController;

- (void) presentImagePickerControllerForRequestId:(uint)requestId
                                       sourceType:(UIImagePickerControllerSourceType)sourceType
                                       mediaTypes:(NSArray<NSString *> *)mediaTypes
                                    allowsEditing:(BOOL)allowsEditing
                                 videoQualityType:(UIImagePickerControllerQualityType)qualityType
                                 maxVideoDuration:(NSTimeInterval)maxVideoDuration
                                     cameraDevice:(UIImagePickerControllerCameraDevice)cameraDevice
                                cameraCaptureMode:(UIImagePickerControllerCameraCaptureMode)cameraCaptureMode
                                        flashMode:(UIImagePickerControllerCameraFlashMode)flashMode
              ipadPopoverPermittedArrowDirections:(UIPopoverArrowDirection)ipadPopoverPermittedArrowDirections
                  ipadNormalizedPopoverSourceRect:(CGRect)ipadNormalizedPopoverSourceRect
                  ipadPopoverCanOverlapSourceRect:(BOOL)ipadPopoverCanOverlapSourceRect;

@end

NS_ASSUME_NONNULL_END
