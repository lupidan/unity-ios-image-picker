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

- (id) filterObjectForSerialization:(id)originalValue
{
    if (!originalValue)
    {
        return nil;
    }
    
    if ([originalValue isKindOfClass:[NSNumber class]] || [originalValue isKindOfClass:[NSString class]])
    {
        return originalValue;
    }
    
    if ([originalValue isKindOfClass:[NSDictionary class]])
    {
        return [self filterDictionaryForSerialization:originalValue];
    }
    
    if ([originalValue isKindOfClass:[NSData class]])
    {
        return [originalValue base64EncodedStringWithOptions:0];
    }
    
    if ([originalValue isKindOfClass:[NSArray class]])
    {
        return [self filterArrayForSerialization:originalValue];
    }
    
    return [originalValue description];
}

- (NSDictionary *) filterDictionaryForSerialization:(NSDictionary *)originalDictionary
{
    if (!originalDictionary)
    {
        return nil;
    }
    
    NSMutableDictionary *filteredDictionary = [NSMutableDictionary dictionary];
    [originalDictionary enumerateKeysAndObjectsUsingBlock:^(id key, id value, BOOL* stop) {
        NSString *filteredKey = [key isKindOfClass:[NSString class]] ? key : [key description];
        id filteredValue = [self filterObjectForSerialization:value];
        
        if (filteredKey && filteredValue)
        {
            [filteredDictionary setObject:filteredValue forKey:filteredKey];
        }
    }];
    
    return [NSDictionary dictionaryWithDictionary:filteredDictionary];
}

- (NSArray *) filterArrayForSerialization:(NSArray *)originalArray
{
    if (!originalArray)
    {
        return nil;
    }
    
    NSMutableArray *filteredArray = [NSMutableArray array];
    [originalArray enumerateObjectsUsingBlock:^(id obj, NSUInteger index, BOOL* stop) {
        id filteredObj = [self filterObjectForSerialization:obj];
        
        if (filteredObj)
        {
            [filteredArray addObject:filteredObj];
        }
    }];
    
    return [NSArray arrayWithArray:filteredArray];
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
    
    NSMutableDictionary *resultPayloadDictionary = [NSMutableDictionary dictionary];
    [resultPayloadDictionary setObject:@false forKey:@"_didCancel"];
    
    NSString *mediaType = [info objectForKey:UIImagePickerControllerMediaType];
    if (mediaType)
    {
        [resultPayloadDictionary setObject:mediaType forKey:@"_mediaType"];
    }
    
    NSMutableDictionary *imageResultPayloadDictionary = [NSMutableDictionary dictionary];
    
    NSValue *cropRectValue = [info objectForKey:UIImagePickerControllerCropRect];
    if (cropRectValue)
    {
        CGRect cropRect = [cropRectValue CGRectValue];
        NSDictionary *cropRectDictionary = @{
            @"_originX" : @(cropRect.origin.x),
            @"_originY" : @(cropRect.origin.y),
            @"_sizeWidth" : @(cropRect.size.width),
            @"_sizeHeight" : @(cropRect.size.height),
        };
        
        [imageResultPayloadDictionary setObject:cropRectDictionary forKey:@"_hasCropRect"];
        [imageResultPayloadDictionary setObject:cropRectDictionary forKey:@"_cropRect"];
    }
    
    UIImage *originalImage = [info objectForKey:UIImagePickerControllerOriginalImage];
    NSString *originalImageFilepath = [self copyImageToTempFolder:originalImage];
    if (originalImageFilepath)
    {
        [imageResultPayloadDictionary setObject:originalImageFilepath forKey:@"_originalImageFileUrl"];
    }
    
    UIImage *editedImage = [info objectForKey:UIImagePickerControllerEditedImage];
    NSString *editedImageFilepath = [self copyImageToTempFolder:editedImage];
    if (editedImageFilepath)
    {
        [imageResultPayloadDictionary setObject:editedImageFilepath forKey:@"_editedImageFileUrl"];
    }
    
    NSString *imageFilepath = nil;
    #if __IPHONE_OS_VERSION_MAX_ALLOWED >= 110000 || __MAC_OS_X_VERSION_MAX_ALLOWED >= 130000
        if (@available(iOS 11.0, *))
        {
            NSURL *imageUrl = [info objectForKey:UIImagePickerControllerImageURL];
            imageFilepath = [imageUrl absoluteString];
        }
    #endif
    if (imageFilepath)
    {
        [imageResultPayloadDictionary setObject:imageFilepath forKey:@"_imageFileUrl"];
    }
    
    if ([imageResultPayloadDictionary count] > 0)
    {
        [resultPayloadDictionary setObject:@YES forKey:@"_containsImage"];
        [resultPayloadDictionary setObject:imageResultPayloadDictionary forKey:@"_image"];
    }
    
    NSMutableDictionary *movieResultPayloadDictionary = [NSMutableDictionary dictionary];
    
    NSURL *mediaUrl = [info objectForKey:UIImagePickerControllerMediaURL];
    NSString *mediaFilepath = [mediaUrl absoluteString];
    if (mediaFilepath)
    {
        [movieResultPayloadDictionary setObject:mediaFilepath forKey:@"_movieFileUrl"];
    }
    
    if ([movieResultPayloadDictionary count] > 0)
    {
        [resultPayloadDictionary setObject:@YES forKey:@"_containsMovie"];
        [resultPayloadDictionary setObject:movieResultPayloadDictionary forKey:@"_movie"];
    }
    
    NSDictionary *filteredMediaMetadata = [self filterObjectForSerialization:[info objectForKey:UIImagePickerControllerMediaMetadata]];
    if (filteredMediaMetadata)
    {
        NSData *filteredMediaMetadataJsonData = [NSJSONSerialization dataWithJSONObject:filteredMediaMetadata options:NULL error:nil];
        NSString *filteredMediaMetadataJsonString = [[NSString alloc] initWithData:filteredMediaMetadataJsonData encoding:NSUTF8StringEncoding];
        if (filteredMediaMetadataJsonString)
        {
            [resultPayloadDictionary setObject:filteredMediaMetadataJsonString forKey:@"_mediaMetadataJson"];
        }
    }
    
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
