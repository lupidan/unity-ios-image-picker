#import <MobileCoreServices/MobileCoreServices.h>
#import "UnityIosImagePickerController.h"

typedef enum
{
    UnityIosImagePickerControllerMediaTypeNone,
    UnityIosImagePickerControllerMediaTypeImage,
    UnityIosImagePickerControllerMediaTypeMovie
} UnityIosImagePickerControllerMediaType;

typedef struct
{
    bool didCancel;
    UnityIosImagePickerControllerMediaType type;
    CGRect cropRect;
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

+ (instancetype) defaultHandler
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

- (void) launchImagePickerControllerForSourceType:(UIImagePickerControllerSourceType)sourceType
                                 videoQualityType:(UIImagePickerControllerQualityType)qualityType
                                     cameraDevice:(UIImagePickerControllerCameraDevice)cameraDevice
                                cameraCaptureMode:(UIImagePickerControllerCameraCaptureMode)cameraCaptureMode
                                        flashMode:(UIImagePickerControllerCameraFlashMode)flashMode
                                 maxVideoDuration:(NSTimeInterval)maxVideoDuration
                                    allowsEditing:(BOOL)allowsEditing
                               showCameraControls:(BOOL)showCameraControls
                                    withRequestId:(int)requestId
{
    UIImagePickerController *imagePickerController = [[UIImagePickerController alloc] init];
    [imagePickerController setDelegate:self];
    [imagePickerController setSourceType:sourceType];
    [imagePickerController setVideoQuality:qualityType];
    [imagePickerController setCameraDevice:cameraDevice];
    [imagePickerController setCameraCaptureMode:cameraCaptureMode];
    [imagePickerController setCameraFlashMode:flashMode];
    [imagePickerController setVideoMaximumDuration:maxVideoDuration];
    [imagePickerController setAllowsEditing:allowsEditing];
    [imagePickerController setShowsCameraControls:showCameraControls];
    
    
    
    NSValue *imagePickerControllerValue = [NSValue valueWithNonretainedObject:imagePickerController];
    [[self requestIdsDictionary] setObject:@(requestId) forKey:imagePickerControllerValue];
}

- (void) bla:(UIImagePickerControllerSourceType) sourceType withRequestId:(int)requestId withCallback:(UnityIosImagePickerControllerCallback)callback
{
    UIImagePickerController *imagePickerController = [[UIImagePickerController alloc] init];
    [imagePickerController setDelegate:self];
    [imagePickerController setSourceType:sourceType];
    

}

- (NSString *) copyImageToTempFolder:(UIImage *)image
{
    if (!image)
    {
        return nil;
    }
    
    NSURL *temporaryDirectory = [NSURL fileURLWithPath:NSTemporaryDirectory() isDirectory:YES];
    NSURL *temporaryUrl = [[NSFileManager defaultManager] URLForDirectory:NSItemReplacementDirectory
                                                                 inDomain:NSUserDomainMask
                                                        appropriateForURL:temporaryDirectory
                                                                   create:YES
                                                                    error:nil]; // TODO ERROR
    
    NSString *filename = [NSString stringWithFormat:@"%@.jpg", [[NSUUID UUID] UUIDString]];
    NSURL *temporaryImageUrl = [temporaryUrl URLByAppendingPathComponent:filename];
    [UIImageJPEGRepresentation(image, 1.0f) writeToURL:temporaryImageUrl atomically:YES];
    return [temporaryUrl absoluteString];
}

#pragma mark - UIImagePickerControllerDelegate implementation

- (void) imagePickerController:(UIImagePickerController *)picker didFinishPickingMediaWithInfo:(NSDictionary<UIImagePickerControllerInfoKey, id> *)info
{
    NSValue *imagePickerControllerValue = [NSValue valueWithNonretainedObject:picker];
    NSNumber *requestIdNumber = [[self requestIdsDictionary] objectForKey:imagePickerControllerValue];
    [[self requestIdsDictionary] removeObjectForKey:imagePickerControllerValue];
    
    if (!requestIdNumber || [self resultCallback] == NULL)
    {
        return;
    }
    
    UnityIosImagePickerControllerResult result;
    result.didCancel = false;

    NSString *mediaType = [info objectForKey:UIImagePickerControllerMediaType];
    if ([mediaType isEqualToString:(NSString *)kUTTypeImage])
    {
        result.type = UnityIosImagePickerControllerMediaTypeImage;
    }
    else if ([mediaType isEqualToString:(NSString *)kUTTypeMovie])
    {
        result.type = UnityIosImagePickerControllerMediaTypeMovie;
    }
    else
    {
        result.type = UnityIosImagePickerControllerMediaTypeNone;
    }
    
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
}

- (void) imagePickerControllerDidCancel:(UIImagePickerController *)picker
{
    NSValue *imagePickerControllerValue = [NSValue valueWithNonretainedObject:picker];
    NSNumber *requestIdNumber = [[self requestIdsDictionary] objectForKey:imagePickerControllerValue];
    [[self requestIdsDictionary] removeObjectForKey:imagePickerControllerValue];
    
    if (!requestIdNumber || [self resultCallback] == NULL)
    {
        return;
    }
    
    UnityIosImagePickerControllerResult result;
    result.didCancel = true;
    result.type = UnityIosImagePickerControllerMediaTypeNone;
    result.cropRect = CGRectZero;
    result.imageUrl = NULL;
    result.originalImageFileUrl = NULL;
    result.editedImageFileUrl = NULL;
    result.videoFileUrl = NULL;
    result.mediaMetadataJson = NULL;
    
    [self resultCallback]([requestIdNumber intValue], result);
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
