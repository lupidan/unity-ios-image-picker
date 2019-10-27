#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface UnityIosImagePickerController : NSObject

+ (instancetype) defaultController;

- (void) presentImagePickerControllerForRequestId:(int)requestId
                                       sourceType:(UIImagePickerControllerSourceType)sourceType
                                       mediaTypes:(NSArray<NSString *> *)mediaTypes
                                    allowsEditing:(BOOL)allowsEditing
                                 videoQualityType:(UIImagePickerControllerQualityType)qualityType
                                 maxVideoDuration:(NSTimeInterval)maxVideoDuration
                               showCameraControls:(BOOL)showCameraControls
      cameraDevice:(UIImagePickerControllerCameraDevice)cameraDevice
                                cameraCaptureMode:(UIImagePickerControllerCameraCaptureMode)cameraCaptureMode
                                        flashMode:(UIImagePickerControllerCameraFlashMode)flashMode;

@end

NS_ASSUME_NONNULL_END

const char* UnityIosImagePickerController_CopyString(const char* string);
