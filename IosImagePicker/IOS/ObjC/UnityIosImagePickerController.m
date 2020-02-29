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

- (void) presentImagePickerControllerForRequestId:(uint)requestId
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

- (NSURL *) setupTempFolder:(NSError **)error
{
    NSURL *mainTempDirectoryUrl = [NSURL fileURLWithPath:NSTemporaryDirectory() isDirectory:YES];
    NSURL *pluginTempDirectoryUrl = [mainTempDirectoryUrl URLByAppendingPathComponent:@"UnityIosImagePicker/" isDirectory:YES];
    
    [[NSFileManager defaultManager] createDirectoryAtURL:pluginTempDirectoryUrl withIntermediateDirectories:YES attributes:nil error:error];
    if (*error)
    {
        return nil;
    }
    
    return pluginTempDirectoryUrl;
}

- (NSString *) cleanupTempFolder:(BOOL)preview
{
    NSURL *mainTempDirectoryUrl = [NSURL fileURLWithPath:NSTemporaryDirectory() isDirectory:YES];
    NSURL *pluginTempDirectoryUrl = [mainTempDirectoryUrl URLByAppendingPathComponent:@"UnityIosImagePicker/" isDirectory:YES];
    NSDirectoryEnumerator<NSURL *> *enumerator = [[NSFileManager defaultManager] enumeratorAtURL:pluginTempDirectoryUrl
                                                                      includingPropertiesForKeys:@[NSURLFileAllocatedSizeKey, NSURLIsDirectoryKey]
                                                                                         options:NULL
                                                                                    errorHandler:nil];
    NSMutableArray *deletionEntries = [NSMutableArray array];
    NSError *error;
    for (NSURL *fileUrl in enumerator)
    {
        NSMutableDictionary *deletionEntryDictionary = [NSMutableDictionary dictionary];
        [deletionEntryDictionary setObject:[fileUrl path] forKey:@"_path"];
        
        NSNumber *isDirectoryNumber = nil;
        [fileUrl getResourceValue:&isDirectoryNumber forKey:NSURLIsDirectoryKey error:&error];
        if (isDirectoryNumber)
        {
            [deletionEntryDictionary setObject:isDirectoryNumber forKey:@"_isDirectory"];
        }
        else if (error)
        {
            NSLog(@"Failed to get NSURLIsDirectoryKey attribute for %@ :: %@", [fileUrl path], error);
        }

        NSNumber *fileSizeNumber = nil;
        [fileUrl getResourceValue:&fileSizeNumber forKey:NSURLFileAllocatedSizeKey error:&error];
        if (fileSizeNumber)
        {
            [deletionEntryDictionary setObject:fileSizeNumber forKey:@"_fileSize"];
        }
        else if (error)
        {
            NSLog(@"Failed to get NSURLFileAllocatedSizeKey attribute for %@ :: %@", [fileUrl path], error);
        }
        
        if (isDirectoryNumber && ![isDirectoryNumber boolValue])
        {
            if (preview)
            {
                [deletionEntryDictionary setObject:@YES forKey:@"_wouldDelete"];
            }
            else
            {
                BOOL deleted = [[NSFileManager defaultManager] removeItemAtURL:fileUrl error:&error];
                if (deleted)
                {
                    [deletionEntryDictionary setObject:@YES forKey:@"_deleted"];
                }
                else if (error)
                {
                    NSLog(@"Failed to remove file at %@ :: %@", [fileUrl path], error);
                }
            }
        }
        
        [deletionEntries addObject:[NSDictionary dictionaryWithDictionary:deletionEntryDictionary]];
    }

    NSDictionary *resultDictionary = @{
        @"_deletionEntries" : [NSArray arrayWithArray:deletionEntries],
    };
    
    NSData *deletionEntriesJsonData = [NSJSONSerialization dataWithJSONObject:resultDictionary options:NULL error:nil];
    NSString *deletionEntriesJsonString = [[NSString alloc] initWithData:deletionEntriesJsonData encoding:NSUTF8StringEncoding];
    return deletionEntriesJsonString;
}

- (NSURL *) writeImageToTempFolder:(UIImage *)image error:(NSError **)error
{
    if (!image)
    {
        return nil;
    }

    NSURL *pluginTempDirectoryUrl = [self setupTempFolder:error];
    if (*error)
    {
        return nil;
    }
    
    NSString *imageFilename = [NSString stringWithFormat:@"%@.png", [[NSUUID UUID] UUIDString]];
    NSURL *imageFileUrl = [pluginTempDirectoryUrl URLByAppendingPathComponent:imageFilename];
    BOOL writeSuccess = [UIImagePNGRepresentation(image) writeToURL:imageFileUrl options:NSDataWritingAtomic error:error];
    if (!writeSuccess || *error)
    {
        return nil;
    }

    return imageFileUrl;
}

- (NSURL *) copyMediaInFileUrlToTempFolder:(NSURL *)fileUrl error:(NSError **)error
{
    if (!fileUrl)
    {
        return nil;
    }
    
    NSURL *pluginTempDirectoryUrl = [self setupTempFolder:error];
    if (*error)
    {
        return nil;
    }
    
    NSString *extension = [fileUrl pathExtension];
    NSString *copiedMediaFilename = [NSString stringWithFormat:@"%@.%@", [[NSUUID UUID] UUIDString], extension];
    NSURL *copiedFileUrl = [pluginTempDirectoryUrl URLByAppendingPathComponent:copiedMediaFilename];
    BOOL copySuccess = [[NSFileManager defaultManager] copyItemAtURL:fileUrl toURL:copiedFileUrl error:error];
    if (!copySuccess || *error)
    {
        return nil;
    }

    return copiedFileUrl;
}

- (void) writeImageResultDataFromReceivedInfo:(NSDictionary<UIImagePickerControllerInfoKey, id> *)info
                        intoPayloadDictionary:(NSMutableDictionary *)payloadDictionary
                                      withKey:(NSString *)key
                                    andHasKey:(NSString *)hasKey
{
    if (!info)
    {
        return;
    }
    
    NSMutableDictionary *imageResultPayloadDictionary = [NSMutableDictionary dictionary];
    
    NSValue *cropRectValue = [info objectForKey:UIImagePickerControllerCropRect];
    [self saveRectValue:cropRectValue
  intoPayloadDictionary:imageResultPayloadDictionary
                withKey:@"_cropRect"
              andHasKey:@"_hasCropRect"];
    
    UIImage *originalImage = [info objectForKey:UIImagePickerControllerOriginalImage];
    [self writeImageToTempFolder:originalImage
andSaveResultIntoPayloadDictionary:imageResultPayloadDictionary
                 withFilePathKey:@"_originalImageFilePath"
                        errorKey:@"_originalImageError"
                  andHasErrorKey:@"_hasOriginalImageError"];
    
    UIImage *editedImage = [info objectForKey:UIImagePickerControllerEditedImage];
    [self writeImageToTempFolder:editedImage
andSaveResultIntoPayloadDictionary:imageResultPayloadDictionary
                 withFilePathKey:@"_editedImageFilePath"
                        errorKey:@"_editedImageError"
                  andHasErrorKey:@"_hasEditedImageError"];
    
    NSURL *imageUrl = nil;
    #if __IPHONE_OS_VERSION_MAX_ALLOWED >= 110000 || __MAC_OS_X_VERSION_MAX_ALLOWED >= 130000
        if (@available(iOS 11.0, *))
        {
            imageUrl = [info objectForKey:UIImagePickerControllerImageURL];
        }
    #endif
    [self copyMediaInFileUrlToTempFolder:imageUrl
      andSaveResultIntoPayloadDictionary:imageResultPayloadDictionary
                         withFilePathKey:@"_imageFilePath"
                                errorKey:@"_imageFileError"
                          andHasErrorKey:@"_hasImageFileError"];
    
    if ([imageResultPayloadDictionary count] > 0)
    {
        [payloadDictionary setObject:@YES forKey:hasKey];
        [payloadDictionary setObject:[NSDictionary dictionaryWithDictionary:imageResultPayloadDictionary] forKey:key];
    }
}

- (void) writeMovieResultDataFromReceivedInfo:(NSDictionary<UIImagePickerControllerInfoKey, id> *)info
                        intoPayloadDictionary:(NSMutableDictionary *)payloadDictionary
                                      withKey:(NSString *)key
                                    andHasKey:(NSString *)hasKey
{
    if (!info)
    {
        return;
    }
    
    NSMutableDictionary *movieResultPayloadDictionary = [NSMutableDictionary dictionary];
    
    NSURL *movieFileUrl = [info objectForKey:UIImagePickerControllerMediaURL];
    [self copyMediaInFileUrlToTempFolder:movieFileUrl
      andSaveResultIntoPayloadDictionary:movieResultPayloadDictionary
                         withFilePathKey:@"_movieFilePath"
                                errorKey:@"_movieFileError"
                          andHasErrorKey:@"_hasMovieFileError"];
    
    if ([movieResultPayloadDictionary count] > 0)
    {
        [payloadDictionary setObject:@YES forKey:hasKey];
        [payloadDictionary setObject:[NSDictionary dictionaryWithDictionary:movieResultPayloadDictionary] forKey:key];
    }
}

- (void) saveJsonRepresentationOfObject:(id)object
                  intoPayloadDictionary:(NSMutableDictionary *)payloadDictionary
                                withKey:(NSString *)key
{
    if (!object)
    {
        return;
    }
    
    NSDictionary *filteredRepresentation = [self filterObjectForSerialization:object];
    if (filteredRepresentation)
    {
        NSData *filteredRepresentationJsonData = [NSJSONSerialization dataWithJSONObject:filteredRepresentation options:NULL error:nil];
        NSString *filteredRepresentationJsonString = [[NSString alloc] initWithData:filteredRepresentationJsonData encoding:NSUTF8StringEncoding];
        [self saveString:filteredRepresentationJsonString intoPayloadDictionary:payloadDictionary withKey:key];
    }
}

- (void) saveRectValue:(NSValue *)rectValue
 intoPayloadDictionary:(NSMutableDictionary *)payloadDictionary
               withKey:(NSString *)key
             andHasKey:(NSString *)hasKey
{
    if (!rectValue)
    {
        return;
    }
    
    CGRect cropRect = [rectValue CGRectValue];
    NSDictionary *cropRectDictionary = @{
        @"_originX" : @(cropRect.origin.x),
        @"_originY" : @(cropRect.origin.y),
        @"_sizeWidth" : @(cropRect.size.width),
        @"_sizeHeight" : @(cropRect.size.height),
    };
    
    [payloadDictionary setObject:@YES forKey:hasKey];
    [payloadDictionary setObject:cropRectDictionary forKey:key];
}

- (void) writeImageToTempFolder:(UIImage *)image
andSaveResultIntoPayloadDictionary:(NSMutableDictionary *)payloadDictionary
                withFilePathKey:(NSString *)filePathKey
                       errorKey:(NSString *)errorKey
                 andHasErrorKey:(NSString *)hasErrorKey
{
    if (!image)
    {
        return;
    }
    
    NSError *writeError = nil;
    NSURL *imageFileUrl = [self writeImageToTempFolder:image error:&writeError];
    [self saveString:[imageFileUrl path] intoPayloadDictionary:payloadDictionary withKey:filePathKey];
    [self saveError:writeError intoPayloadDictionary:payloadDictionary withErrorKey:errorKey andHasErrorKey:hasErrorKey];
}

- (void) copyMediaInFileUrlToTempFolder:(NSURL *)originalMediaFileUrl
     andSaveResultIntoPayloadDictionary:(NSMutableDictionary *)payloadDictionary
                        withFilePathKey:(NSString *)filePathKey
                               errorKey:(NSString *)errorKey
                         andHasErrorKey:(NSString *)hasErrorKey
{
    if (!originalMediaFileUrl)
    {
        return;
    }
    
    NSError *copyError = nil;
    NSURL *copiedMediaFileUrl = [self copyMediaInFileUrlToTempFolder:originalMediaFileUrl error:&copyError];
    [self saveString:[copiedMediaFileUrl path] intoPayloadDictionary:payloadDictionary withKey:filePathKey];
    [self saveError:copyError intoPayloadDictionary:payloadDictionary withErrorKey:errorKey andHasErrorKey:hasErrorKey];
}

- (void) saveError:(NSError *)error intoPayloadDictionary:(NSMutableDictionary *)payloadDictionary withErrorKey:(NSString *)errorKey andHasErrorKey:(NSString *)hasErrorKey
{
    if (!error)
    {
        return;
    }
    
    NSMutableDictionary *errorDictionary = [NSMutableDictionary dictionary];
    [errorDictionary setObject:@([error code]) forKey:@"_code"];
    [self saveString:[error domain] intoPayloadDictionary:errorDictionary withKey:@"_domain"];
    [self saveString:[error localizedDescription] intoPayloadDictionary:errorDictionary withKey:@"_localizedDescription"];
    [payloadDictionary setObject:@YES forKey:hasErrorKey];
    [payloadDictionary setObject:[NSDictionary dictionaryWithDictionary:errorDictionary] forKey:errorKey];
}

- (void) saveString:(NSString *)valueString intoPayloadDictionary:(NSMutableDictionary *)payloadDictionary withKey:(NSString *)key
{
    if (!valueString)
    {
        return;
    }
    
    [payloadDictionary setObject:valueString forKey:key];
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
    [resultPayloadDictionary setObject:@NO forKey:@"_didCancel"];
    
    NSString *mediaType = [info objectForKey:UIImagePickerControllerMediaType];
    id mediaMetadata = [info objectForKey:UIImagePickerControllerMediaMetadata];
    [self saveString:mediaType intoPayloadDictionary:resultPayloadDictionary withKey:@"_mediaType"];
    [self writeImageResultDataFromReceivedInfo:info intoPayloadDictionary:resultPayloadDictionary withKey:@"_image" andHasKey:@"_hasImage"];
    [self writeMovieResultDataFromReceivedInfo:info intoPayloadDictionary:resultPayloadDictionary withKey:@"_movie" andHasKey:@"_hasMovie"];
    [self saveJsonRepresentationOfObject:mediaMetadata intoPayloadDictionary:resultPayloadDictionary withKey:@"_mediaMetadataJson"];
    
    NSData *resultPayloadJsonData = [NSJSONSerialization dataWithJSONObject:resultPayloadDictionary options:NULL error:nil];
    NSString *resultPayloadJsonString = [[NSString alloc] initWithData:resultPayloadJsonData encoding:NSUTF8StringEncoding];
    [self resultCallback]([requestIdNumber unsignedIntValue], [resultPayloadJsonString UTF8String]);
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
        @"_didCancel": @YES,
    };
    
    NSData *resultPayloadJsonData = [NSJSONSerialization dataWithJSONObject:resultPayloadDictionary options:NULL error:nil];
    NSString *resultPayloadJsonString = [[NSString alloc] initWithData:resultPayloadJsonData encoding:NSUTF8StringEncoding];
    [self resultCallback]([requestIdNumber unsignedIntValue], [resultPayloadJsonString UTF8String]);
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

const char* UnityIosImagePickerController_CleanupTempFolder(bool preview)
{
    NSString *deletionEntriesJson = [[UnityIosImagePickerController defaultController] cleanupTempFolder:preview ? YES : NO];
    return UnityIosImagePickerController_CopyString([deletionEntriesJson UTF8String]);
}

void UnityIosImagePickerController_Present(
    uint requestId,
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
