#import <MobileCoreServices/MobileCoreServices.h>
#import "UnityIosImagePickerController.h"

typedef struct
{
    bool didCancel;
    CGRect cropRect;
    const char *mediaType;
    const char *imageUrl;
    const char *originalImageFileUrl;
    const char *editedImageFileUrl;
    const char *videoFileUrl;
    const char *mediaMetadataJson;
} UnityIosImagePickerControllerResult;

typedef void (*UnityIosImagePickerControllerCallback)(int requestId, UnityIosImagePickerControllerResult result);

@interface UnityIosImagePickerController () <UINavigationControllerDelegate, UIImagePickerControllerDelegate>
@property(nonatomic, strong) NSMutableDictionary *requestIdsDictionary;
@property(nonatomic, assign) UnityIosImagePickerControllerCallback resultCallback;
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
    
    UnityIosImagePickerControllerResult result;
    result.didCancel = false;

    NSString *mediaType = [info objectForKey:UIImagePickerControllerMediaType];
    result.mediaType = UnityIosImagePickerController_CopyString([mediaType UTF8String]);
    
    NSValue *cropRectValue = [info objectForKey:UIImagePickerControllerCropRect];
    result.cropRect = cropRectValue ? [cropRectValue CGRectValue] : CGRectZero;
    
#if __IPHONE_OS_VERSION_MAX_ALLOWED >= 110000 || __MAC_OS_X_VERSION_MAX_ALLOWED >= 130000
    if (@available(iOS 11.0, *))
    {
        NSURL *imageUrl = [info objectForKey:UIImagePickerControllerImageURL];
        result.imageUrl = UnityIosImagePickerController_CopyString([[imageUrl absoluteString] UTF8String]);
    }
    else
    {
        result.imageUrl = NULL;
    }
#else
    result.imageUrl = NULL;
#endif
    
    UIImage *originalImage = [info objectForKey:UIImagePickerControllerOriginalImage];
    NSString *originalImageFilepath = [self copyImageToTempFolder:originalImage];
    result.originalImageFileUrl = UnityIosImagePickerController_CopyString([originalImageFilepath UTF8String]);

    UIImage *editedImage = [info objectForKey:UIImagePickerControllerEditedImage];
    NSString *editedImageFilepath = [self copyImageToTempFolder:editedImage];
    result.editedImageFileUrl = UnityIosImagePickerController_CopyString([editedImageFilepath UTF8String]);
    
    NSURL *mediaUrl = [info objectForKey:UIImagePickerControllerMediaURL];
    result.videoFileUrl = UnityIosImagePickerController_CopyString([[mediaUrl absoluteString] UTF8String]);
    
    NSDictionary *mediaMetadata = [info objectForKey:UIImagePickerControllerMediaMetadata];
    NSData *mediaMetadataJsonData = mediaMetadata ? [NSJSONSerialization dataWithJSONObject:mediaMetadata options:NULL error:nil] : nil;
    NSString *mediaMetadataJsonString = mediaMetadataJsonData ? [[NSString alloc] initWithData:mediaMetadataJsonData encoding:NSUTF8StringEncoding] : nil;
    result.mediaMetadataJson = UnityIosImagePickerController_CopyString([mediaMetadataJsonString UTF8String]);
    
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
    
    UnityIosImagePickerControllerResult result;
    result.didCancel = true;
    result.mediaType = NULL;
    result.cropRect = CGRectZero;
    result.imageUrl = NULL;
    result.originalImageFileUrl = NULL;
    result.editedImageFileUrl = NULL;
    result.videoFileUrl = NULL;
    result.mediaMetadataJson = NULL;
    
    [self resultCallback]([requestIdNumber intValue], result);
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
