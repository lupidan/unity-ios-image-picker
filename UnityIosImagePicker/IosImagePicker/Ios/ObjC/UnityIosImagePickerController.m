#import <MobileCoreServices/MobileCoreServices.h>
#import "UnityIosImagePickerController.h"

typedef void (*UnityIosImagePickerControllerResultDelegate)(int requestId, const char* resultPayload);

@interface UnityIosImagePickerController () <UINavigationControllerDelegate, UIImagePickerControllerDelegate>
@property(nonatomic, strong) NSMutableDictionary *requestIdsDictionary;
@property(nonatomic, assign) UnityIosImagePickerControllerResultDelegate resultCallback;
@end

@implementation UnityIosImagePickerController

+ (instancetype) defaultController
{
    static UnityIosImagePickerController *_defaultController = nil;
    static dispatch_once_t defaultControllerInitialization;
    
    dispatch_once(&defaultControllerInitialization, ^{
        _defaultController = [[UnityIosImagePickerController alloc] init];
    });
    
    return _defaultController;
}

- (instancetype) init
{
    self = [super init];
    if (self)
    {
        _requestIdsDictionary = [[NSMutableDictionary alloc] init];
    }
    return self;
}

- (void) presentImagePickerControllerForRequestId:(int)requestId
                                       sourceType:(UIImagePickerControllerSourceType)sourceType
                                        mediaTypes:(NSArray<NSString *> *)mediaTypes
                                    allowsEditing:(BOOL)allowsEditing
                                 videoQualityType:(UIImagePickerControllerQualityType)qualityType
                                 maxVideoDuration:(NSTimeInterval)maxVideoDuration
                                     cameraDevice:(UIImagePickerControllerCameraDevice)cameraDevice
                                cameraCaptureMode:(UIImagePickerControllerCameraCaptureMode)cameraCaptureMode
                                        flashMode:(UIImagePickerControllerCameraFlashMode)flashMode
{
    UIImagePickerController *imagePickerController = [[UIImagePickerController alloc] init];
    [imagePickerController setDelegate:self];
    [imagePickerController setSourceType:sourceType];
    [imagePickerController setMediaTypes:mediaTypes];
    [imagePickerController setAllowsEditing:allowsEditing];
    [imagePickerController setVideoQuality:qualityType];
    [imagePickerController setVideoMaximumDuration:maxVideoDuration];
    
    if (sourceType == UIImagePickerControllerSourceTypeCamera)
    {
        [imagePickerController setCameraDevice:cameraDevice];
        [imagePickerController setCameraCaptureMode:cameraCaptureMode];
        [imagePickerController setCameraFlashMode:flashMode];
    }
    
    NSValue *imagePickerControllerValue = [NSValue valueWithNonretainedObject:imagePickerController];
    [[self requestIdsDictionary] setObject:@(requestId) forKey:imagePickerControllerValue];
    
#if __IPHONE_OS_VERSION_MAX_ALLOWED >= 80000 || __MAC_OS_X_VERSION_MAX_ALLOWED >= 130000
    if (@available(iOS 8.0, *))
    {
        [self presentImagePickerController:imagePickerController];
    }
    else
    {
        [self legacyPresentImagePickerController:imagePickerController];
    }
#else
    [self legacyPresentImagePickerController:imagePickerController];
#endif
}

- (void) presentImagePickerController:(UIImagePickerController *)imagePickerController
{
    UIViewController *rootViewController = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
    
    // On iPad, UIImagePickerControllerSourceTypePhotoLibrary and UIImagePickerControllerSourceTypeSavedPhotosAlbum
    // must be presented in a popover
    if ([[UIDevice currentDevice] userInterfaceIdiom] == UIUserInterfaceIdiomPad &&
        [imagePickerController sourceType] != UIImagePickerControllerSourceTypeCamera)
    {
        [imagePickerController setModalPresentationStyle:UIModalPresentationPopover];
        [[imagePickerController popoverPresentationController] setSourceRect:CGRectZero];
        [[imagePickerController popoverPresentationController] setSourceView:[rootViewController view]];
    } else {
        [rootViewController setModalPresentationStyle:UIModalPresentationFullScreen];
    }
    
    [rootViewController presentViewController:imagePickerController animated:YES completion:nil];
}

- (void) legacyPresentImagePickerController:(UIImagePickerController *)imagePickerController
{
    UIViewController *rootViewController = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
    
    if ([[UIDevice currentDevice] userInterfaceIdiom] == UIUserInterfaceIdiomPad &&
        [imagePickerController sourceType] != UIImagePickerControllerSourceTypeCamera)
    {
        UIPopoverController *popoverController = [[UIPopoverController alloc] initWithContentViewController:imagePickerController];
        [popoverController presentPopoverFromRect:CGRectZero
                                           inView:[rootViewController view]
                         permittedArrowDirections:UIPopoverArrowDirectionAny
                                         animated:YES];
    } else {
        [rootViewController presentViewController:imagePickerController animated:YES completion:nil];
    }
}

- (NSString *) copyImageToTempFolder:(UIImage *)image
{
    if (!image)
    {
        return nil;
    }
    
    NSURL *mainTempDirectoryUrl = [NSURL fileURLWithPath:NSTemporaryDirectory() isDirectory:YES];
    NSURL *pluginTempDirectoryUrl = [mainTempDirectoryUrl URLByAppendingPathComponent:@"UnityIosImagePicker/" isDirectory:YES];
    [[NSFileManager defaultManager] createDirectoryAtURL:pluginTempDirectoryUrl withIntermediateDirectories:YES attributes:nil error:nil];
    NSString *filename = [NSString stringWithFormat:@"%@.jpg", [[NSUUID UUID] UUIDString]];
    NSURL *tempImageUrl = [pluginTempDirectoryUrl URLByAppendingPathComponent:filename];
    BOOL writeSuccess = [UIImageJPEGRepresentation(image, 1.0f) writeToURL:tempImageUrl atomically:YES];
    return writeSuccess ? [tempImageUrl absoluteString] : nil;
}

#pragma mark - UIImagePickerControllerDelegate implementation

- (void) imagePickerController:(UIImagePickerController *)picker didFinishPickingMediaWithInfo:(NSDictionary<UIImagePickerControllerInfoKey, id> *)info
{
    NSValue *imagePickerControllerValue = [NSValue valueWithNonretainedObject:picker];
    NSNumber *requestIdNumber = [[self requestIdsDictionary] objectForKey:imagePickerControllerValue];
    [[self requestIdsDictionary] removeObjectForKey:imagePickerControllerValue];
    
    if (!requestIdNumber || [self resultCallback] == NULL)
    {
        [picker dismissViewControllerAnimated:YES completion:nil];
        return;
    }
    
    NSString *mediaType = [info objectForKey:UIImagePickerControllerMediaType];
    
    NSValue *cropRectValue = [info objectForKey:UIImagePickerControllerCropRect];
    
    UIImage *originalImage = [info objectForKey:UIImagePickerControllerOriginalImage];
    NSString *originalImageFilepath = [self copyImageToTempFolder:originalImage];
    
    UIImage *editedImage = [info objectForKey:UIImagePickerControllerEditedImage];
    NSString *editedImageFilepath = [self copyImageToTempFolder:editedImage];
    
    NSURL *mediaUrl = [info objectForKey:UIImagePickerControllerMediaURL];
    NSString *mediaFilepath = [mediaUrl absoluteString];
    
    NSDictionary *mediaMetadata = [info objectForKey:UIImagePickerControllerMediaMetadata];
    
    NSString *imageFilepath = nil;
#if __IPHONE_OS_VERSION_MAX_ALLOWED >= 110000 || __MAC_OS_X_VERSION_MAX_ALLOWED >= 130000
    if (@available(iOS 11.0, *))
    {
        NSURL *imageUrl = [info objectForKey:UIImagePickerControllerImageURL];
        imageFilepath = [imageUrl absoluteString];
    }
#endif
    
    NSDictionary *resultPayloadDictionary = @{
        @"didCancel": @false,
        @"mediaType": mediaType ? mediaType : @"",
        @"cropRect": cropRectValue ? cropRectValue : @"",
        @"originalImageFilepath": originalImageFilepath ? originalImageFilepath : @"",
        @"editedImageFilepath": editedImageFilepath ? editedImageFilepath : @"",
        @"mediaFilepath": mediaFilepath ? mediaFilepath : @"",
        @"imageFilepath": imageFilepath ? imageFilepath : @"",
        @"mediaMetadata": mediaMetadata ? mediaMetadata : @"{}",
    };
    
    NSData *resultPayloadJsonData = [NSJSONSerialization dataWithJSONObject:resultPayloadDictionary options:NULL error:nil];
    NSString *resultPayloadJsonString = [[NSString alloc] initWithData:resultPayloadJsonData encoding:NSUTF8StringEncoding];
    [self resultCallback]([requestIdNumber intValue], [resultPayloadJsonString UTF8String]);
    [picker dismissViewControllerAnimated:YES completion:nil];
}

- (void) imagePickerControllerDidCancel:(UIImagePickerController *)picker
{
    NSValue *imagePickerControllerValue = [NSValue valueWithNonretainedObject:picker];
    NSNumber *requestIdNumber = [[self requestIdsDictionary] objectForKey:imagePickerControllerValue];
    [[self requestIdsDictionary] removeObjectForKey:imagePickerControllerValue];
    
    if (!requestIdNumber || [self resultCallback] == NULL)
    {
        [picker dismissViewControllerAnimated:YES completion:nil];
        return;
    }
    
    NSDictionary *resultPayloadDictionary = @{
        @"didCancel": @true,
        @"mediaType": @"",
        @"cropRect": @"",
        @"originalImageFilepath": @"",
        @"editedImageFilepath": @"",
        @"mediaFilepath": @"",
        @"imageFilepath": @"",
        @"mediaMetadata": @"{}",
    };
    
    NSData *resultPayloadJsonData = [NSJSONSerialization dataWithJSONObject:resultPayloadDictionary options:NULL error:nil];
    NSString *resultPayloadJsonString = [[NSString alloc] initWithData:resultPayloadJsonData encoding:NSUTF8StringEncoding];
    [self resultCallback]([requestIdNumber intValue], [resultPayloadJsonString UTF8String]);
    [picker dismissViewControllerAnimated:YES completion:nil];
}

@end

#pragma mark - Native C Calls

const char* UnityIosImagePickerController_CopyString(const char* string)
{
    if (string == NULL)
    {
        return NULL;
    }

    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);

    return res;
}

void UnityIosImagePickerController_SetResultCallback(UnityIosImagePickerControllerResultDelegate resultCallback)
{
    [[UnityIosImagePickerController defaultController] setResultCallback:resultCallback];
}

const char* UnityIosImagePickerController_GetMediaTypeImage()
{
    NSString *mediaType = (NSString *)kUTTypeImage;
    return UnityIosImagePickerController_CopyString([mediaType UTF8String]);
}

const char* UnityIosImagePickerController_GetMediaTypeMovie()
{
    NSString *mediaType = (NSString *)kUTTypeMovie;
    return UnityIosImagePickerController_CopyString([mediaType UTF8String]);
}

bool UnityIosImagePickerController_IsSourceTypeAvailable(int sourceType)
{
    return [UIImagePickerController isSourceTypeAvailable:sourceType];
}

bool UnityIosImagePickerController_IsCameraDeviceAvailable(int cameraDevice)
{
    return [UIImagePickerController isCameraDeviceAvailable:cameraDevice];
}

const char* UnityIosImagePickerController_AvailableMediaTypesForSourceType(int sourceType)
{
    NSArray<NSString *> *mediaTypes = [UIImagePickerController availableMediaTypesForSourceType:sourceType];
    NSString* serializedMediaTypes = [mediaTypes componentsJoinedByString:@"#"];
    return UnityIosImagePickerController_CopyString([serializedMediaTypes UTF8String]);
}

const char* UnityIosImagePickerController_AvailableCaptureModesForCameraDevice(int cameraDevice)
{
    NSArray<NSNumber *> *captureModes = [UIImagePickerController availableCaptureModesForCameraDevice:cameraDevice];
    NSString* serializedCaptureModes = [captureModes componentsJoinedByString:@"#"];
    return UnityIosImagePickerController_CopyString([serializedCaptureModes UTF8String]);
}

bool UnityIosImagePickerController_IsFlashAvailableForCameraDevice(int cameraDevice)
{
    return [UIImagePickerController isFlashAvailableForCameraDevice:cameraDevice];
}

void UnityIosImagePickerController_Present(
    int requestId,
    int sourceType,
    const char* serializedMediaTypes,
    bool allowsEditing,
    int videoQuality,
    double videoMaximumDurationInSeconds,
    int cameraDevice,
    int cameraCaptureMode,
    int cameraFlashMode)
{
    NSString *serializedMediaTypesString = [NSString stringWithUTF8String:serializedMediaTypes];
    NSArray <NSString *> *mediaTypes = [serializedMediaTypesString componentsSeparatedByString:@"#"];
    UnityIosImagePickerController *defaultController = [UnityIosImagePickerController defaultController];
    [defaultController presentImagePickerControllerForRequestId:requestId
                                                     sourceType:sourceType
                                                     mediaTypes:mediaTypes
                                                  allowsEditing:allowsEditing ? YES : NO
                                               videoQualityType:videoQuality
                                               maxVideoDuration:videoMaximumDurationInSeconds
                                                   cameraDevice:cameraDevice
                                              cameraCaptureMode:cameraCaptureMode
                                                      flashMode:cameraFlashMode];
}
