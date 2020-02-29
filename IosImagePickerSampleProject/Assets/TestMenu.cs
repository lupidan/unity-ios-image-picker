using System;
using System.Collections.Generic;
using DefaultNamespace;
using IosImagePicker;
using IosImagePicker.Enums;
using UnityEngine;
using UnityEngine.UI;
using IosImagePicker.Interfaces;
using IosImagePicker.IOS.NativeMessages;
using System.IO;
using System.Text;

public class TestMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject sourceTypeRowGameObject;
    [SerializeField]
    private Dropdown sourceTypeDropdown;
    [Space]
    [SerializeField]
    private Toggle imageTypeToggle;
    [SerializeField]
    private Text imageTypeText;
    [SerializeField]
    private Toggle movieTypeToggle;
    [SerializeField]
    private Text movieTypeText;
    [Space]
    [SerializeField]
    private GameObject cameraDeviceRowGameObject;
    [SerializeField]
    private Dropdown cameraDeviceDropdown;
    [Space]
    [SerializeField]
    private GameObject cameraCaptureModeRowGameObject;
    [SerializeField]
    private Dropdown cameraCaptureModeDropdown;
    [Space]
    [SerializeField]
    private GameObject cameraFlashModeRowGameObject;
    [SerializeField]
    private Dropdown cameraFlashModeDropdown;
    [Space]
    [SerializeField]
    private GameObject allowsEditingRowGameObject;
    [SerializeField]
    private Dropdown allowsEditingDropdown;
    [Space]
    [SerializeField]
    private GameObject videoQualityRowGameObject;
    [SerializeField]
    private Dropdown videoQualityDropdown;
    [Space]
    [SerializeField]
    private GameObject videoMaxDurationRowGameObject;
    [SerializeField]
    private Dropdown videoMaxDurationDropdown;

    private IIosImagePicker _iosImagePicker;
    private List<IInterfaceController> _interfaceControllers;

    private void Start()
    {
        var iosImagePicker = default(IIosImagePicker);
        if (NativeIosImagePicker.IsCurrentPlatformSupported)
        {
            var payloadDeserializer = new PayloadDeserializer();
            iosImagePicker = new NativeIosImagePicker(payloadDeserializer);    
        }
        
        var interfaceControllers = new List<IInterfaceController>();
        
        var sourceTypeDropdownController = SetupSourceTypeDropdown(iosImagePicker, this.sourceTypeRowGameObject, this.sourceTypeDropdown);
        interfaceControllers.Add(sourceTypeDropdownController);

        var mediaTypesToggleGroupController = SetupMediaTypesToggleGroup(iosImagePicker, this.imageTypeText, this.movieTypeText, this.imageTypeToggle, this.movieTypeToggle, sourceTypeDropdownController);
        interfaceControllers.Add(mediaTypesToggleGroupController);
        
        var cameraDeviceDropdownController = SetupCameraDeviceDropdown(iosImagePicker, this.cameraDeviceRowGameObject, this.cameraDeviceDropdown);
        interfaceControllers.Add(cameraDeviceDropdownController);
        
        var cameraCaptureModeDropdownController = SetupCameraCaptureModeDropdown(iosImagePicker, this.cameraCaptureModeRowGameObject, this.cameraCaptureModeDropdown, cameraDeviceDropdownController);
        interfaceControllers.Add(cameraCaptureModeDropdownController);
        
        var cameraFlashModeDropdownController = SetupCameraFlashModeDropdown(iosImagePicker, this.cameraFlashModeRowGameObject, this.cameraFlashModeDropdown, cameraDeviceDropdownController);
        interfaceControllers.Add(cameraFlashModeDropdownController);
        
        var allowsEditingDropdownController = SetupAllowsEditingDropdown(iosImagePicker, this.allowsEditingRowGameObject, this.allowsEditingDropdown);
        interfaceControllers.Add(allowsEditingDropdownController);
        
        var videoQualityDropdownController = SetupVideoQualityDropdown(iosImagePicker, this.videoQualityRowGameObject, this.videoQualityDropdown, mediaTypesToggleGroupController);
        interfaceControllers.Add(videoQualityDropdownController);
        
        var videoMaxDurationDropdownController = SetupVideoMaxDurationDropdown(iosImagePicker, this.videoMaxDurationRowGameObject, this.videoMaxDurationDropdown, mediaTypesToggleGroupController);
        interfaceControllers.Add(videoMaxDurationDropdownController);

        for (var i = 0; i < interfaceControllers.Count; i++)
        {
            interfaceControllers[i].Refresh();
        }

        this._iosImagePicker = iosImagePicker;
        this._interfaceControllers = interfaceControllers;
    }

    private void Update()
    {
        if (this._iosImagePicker != null)
        {
            this._iosImagePicker.Update();
        }
    }

    private static DropdownController<IosImagePickerSourceType> SetupSourceTypeDropdown(
        IIosImagePicker iosImagePicker,
        GameObject rowGameObject,
        Dropdown dropdown)
    {
        var allSourceTypes = new[]
        {
            IosImagePickerSourceType.PhotoLibrary,
            IosImagePickerSourceType.Camera,
            IosImagePickerSourceType.SavedPhotosAlbum,
        };
        
        var valueVisibilityFilter = default(Func<IosImagePickerSourceType, bool>);
        if (iosImagePicker is NativeIosImagePicker)
        {
            valueVisibilityFilter = NativeIosImagePicker.IsSourceTypeAvailable;
        }
        
        var dropdownController = new DropdownController<IosImagePickerSourceType>(
            allSourceTypes,
            rowGameObject,
            dropdown,
            () => iosImagePicker.SourceType,
            sourceType => iosImagePicker.SourceType = sourceType,
            valueVisibilityFilter);
        
        dropdownController.Setup();
        return dropdownController;
    }

    private static DropdownController<IosImagePickerCameraDevice> SetupCameraDeviceDropdown(
        IIosImagePicker iosImagePicker,
        GameObject rowGameObject,
        Dropdown dropdown)
    {
        var allCameraDevices = new[]
        {
            IosImagePickerCameraDevice.Front,
            IosImagePickerCameraDevice.Rear,
        };
        
        var valueVisibilityFilter = default(Func<IosImagePickerCameraDevice, bool>);
        if (iosImagePicker is NativeIosImagePicker)
        {
            valueVisibilityFilter = NativeIosImagePicker.IsCameraDeviceAvailable;
        }

        var dropdownController = new DropdownController<IosImagePickerCameraDevice>(
            allCameraDevices,
            rowGameObject,
            dropdown,
            () => iosImagePicker.CameraDevice,
            cameraDevice => iosImagePicker.CameraDevice = cameraDevice,
            valueVisibilityFilter);
        
        dropdownController.Setup();
        return dropdownController;
    }
    
    private static DropdownController<IosImagePickerCameraCaptureMode> SetupCameraCaptureModeDropdown(
        IIosImagePicker iosImagePicker,
        GameObject rowGameObject,
        Dropdown dropdown,
        DropdownController<IosImagePickerCameraDevice> cameraDeviceDropdownController)
    {
        var allCaptureModes = new[]
        {
            IosImagePickerCameraCaptureMode.Photo,
            IosImagePickerCameraCaptureMode.Video,
        };
        
        var valueVisibilityFilter = default(Func<IosImagePickerCameraCaptureMode, bool>);
        if (iosImagePicker is NativeIosImagePicker)
        {
            valueVisibilityFilter = captureMode =>
            {
                var currentCameraDevice = iosImagePicker.CameraDevice;
                var availableCaptureModes = NativeIosImagePicker.AvailableCaptureModesForCameraDevice(currentCameraDevice);
                return availableCaptureModes != null && Array.IndexOf(availableCaptureModes, captureMode) > -1;
            };
        }

        var dropdownController = new DropdownController<IosImagePickerCameraCaptureMode>(
            allCaptureModes,
            rowGameObject,
            dropdown,
            () => iosImagePicker.CameraCaptureMode,
            captureMode => iosImagePicker.CameraCaptureMode = captureMode,
            valueVisibilityFilter);
        
        dropdownController.Setup();
        cameraDeviceDropdownController.AddDependantController(dropdownController);
        return dropdownController;
    }
    
    private static DropdownController<IosImagePickerCameraFlashMode> SetupCameraFlashModeDropdown(
        IIosImagePicker iosImagePicker,
        GameObject rowGameObject,
        Dropdown dropdown,
        DropdownController<IosImagePickerCameraDevice> cameraDeviceDropdownController)
    {
        var allFlashModes = new[]
        {
            IosImagePickerCameraFlashMode.Auto,
            IosImagePickerCameraFlashMode.On,
            IosImagePickerCameraFlashMode.Off,
        };
        
        var valueVisibilityFilter = default(Func<IosImagePickerCameraFlashMode, bool>);
        if (iosImagePicker is NativeIosImagePicker)
        {
            valueVisibilityFilter = flashMode =>
            {
                var currentCameraDevice = iosImagePicker.CameraDevice;
                return flashMode != IosImagePickerCameraFlashMode.On || NativeIosImagePicker.IsFlashAvailableForCameraDevice(currentCameraDevice);
            };
        }

        var dropdownController = new DropdownController<IosImagePickerCameraFlashMode>(
            allFlashModes,
            rowGameObject,
            dropdown,
            () => iosImagePicker.CameraFlashMode,
            flashMode => iosImagePicker.CameraFlashMode = flashMode,
            valueVisibilityFilter);

        dropdownController.Setup();
        cameraDeviceDropdownController.AddDependantController(dropdownController);
        return dropdownController;
    }
    
    private static DropdownController<bool> SetupAllowsEditingDropdown(
        IIosImagePicker iosImagePicker,
        GameObject rowGameObject,
        Dropdown dropdown)
    {
        var allValues = new[]
        {
            false,
            true,
        };

        var dropdownController = new DropdownController<bool>(
            allValues,
            rowGameObject,
            dropdown,
            () => iosImagePicker.AllowsEditing,
            allowsEditing => iosImagePicker.AllowsEditing = allowsEditing,
            null);
        
        dropdownController.Setup();
        return dropdownController;
    }
    
    private static DropdownController<IosImagePickerVideoQualityType> SetupVideoQualityDropdown(
        IIosImagePicker iosImagePicker,
        GameObject rowGameObject,
        Dropdown dropdown,
        ToggleGroupController<string> mediaTypesToggleGroupController)
    {
        var allQualityTypes = new[]
        {
            IosImagePickerVideoQualityType.High,
            IosImagePickerVideoQualityType.Medium,
            IosImagePickerVideoQualityType.Low,
            IosImagePickerVideoQualityType.VGA640x480,
            IosImagePickerVideoQualityType.IFrame1280x720,
            IosImagePickerVideoQualityType.IFrame960x540,
        };
        
        var valueVisibilityFilter = default(Func<IosImagePickerVideoQualityType, bool>);
        if (iosImagePicker is NativeIosImagePicker)
        {
            valueVisibilityFilter = qualityType =>
            {
                var currentMediaTypes = iosImagePicker.MediaTypes;
                return currentMediaTypes != null && Array.IndexOf(currentMediaTypes, iosImagePicker.MediaTypeMovie) > -1;
            };
        }

        var dropdownController = new DropdownController<IosImagePickerVideoQualityType>(
            allQualityTypes,
            rowGameObject,
            dropdown,
            () => iosImagePicker.VideoQuality,
            videoQuality => iosImagePicker.VideoQuality = videoQuality,
            valueVisibilityFilter);
        
        dropdownController.Setup();
        mediaTypesToggleGroupController.AddDependantController(dropdownController);
        return dropdownController;
    }
    
    private static DropdownController<TimeSpan> SetupVideoMaxDurationDropdown(
        IIosImagePicker iosImagePicker,
        GameObject rowGameObject,
        Dropdown dropdown,
        ToggleGroupController<string> mediaTypesToggleGroupController)
    {
        var allMaxDurations = new[]
        {
            TimeSpan.FromSeconds(30.0),
            TimeSpan.FromSeconds(200.0),
            TimeSpan.FromSeconds(600.0),
        };

        iosImagePicker.VideoMaximumDuration = allMaxDurations[0];
        
        var valueVisibilityFilter = default(Func<TimeSpan, bool>);
        if (iosImagePicker is NativeIosImagePicker)
        {
            valueVisibilityFilter = maxDuration =>
            {
                var currentMediaTypes = iosImagePicker.MediaTypes;
                return currentMediaTypes != null && Array.IndexOf(currentMediaTypes, iosImagePicker.MediaTypeMovie) > -1;
            };
        }
        
        var dropdownController = new DropdownController<TimeSpan>(
            allMaxDurations,
            rowGameObject,
            dropdown,
            () => iosImagePicker.VideoMaximumDuration,
            maxDuration => iosImagePicker.VideoMaximumDuration = maxDuration,
            valueVisibilityFilter);
        
        dropdownController.Setup();
        mediaTypesToggleGroupController.AddDependantController(dropdownController);
        return dropdownController;
    }

    private static ToggleGroupController<string> SetupMediaTypesToggleGroup(
        IIosImagePicker iosImagePicker,
        Text imageTypeText,
        Text movieTypeText,
        Toggle imageTypeToggle,
        Toggle movieTypeToggle,
        DropdownController<IosImagePickerSourceType> sourceTypeDropdownController)
    {
        var values = new[]
        {
            iosImagePicker.MediaTypeImage,
            iosImagePicker.MediaTypeMovie,
        };

        var labels = new[]
        {
            imageTypeText,
            movieTypeText,
        };

        var toggles = new[]
        {
            imageTypeToggle,
            movieTypeToggle,
        };
        
        var valueVisibilityFilter = default(Func<string, bool>);
        if (iosImagePicker is NativeIosImagePicker)
        {
            valueVisibilityFilter = mediaType =>
            {
                var availableMediaTypes = NativeIosImagePicker.AvailableMediaTypesForSourceType(iosImagePicker.SourceType);
                return availableMediaTypes != null && Array.IndexOf(availableMediaTypes, mediaType) > -1;
            };
        }

        var toggleGroupController = new ToggleGroupController<string>(
            values,
            labels,
            toggles,
            () => iosImagePicker.MediaTypes,
            mediaTypes => iosImagePicker.MediaTypes = mediaTypes,
            valueVisibilityFilter);
        
        toggleGroupController.Setup();
        sourceTypeDropdownController.AddDependantController(toggleGroupController);
        return toggleGroupController;
    }

    public void Present()
    {
        this._iosImagePicker.Present(result =>
        {
            var stringBuilder = new StringBuilder("RESULT:\n");
            stringBuilder.AppendLine("Was Cancelled: " + result.DidCancel);
            stringBuilder.AppendLine("Media Type: " + result.MediaType);
            stringBuilder.AppendLine("Media Metadata: " + result.MediaMetadataJson);
            if (result.Image != null)
            {
                LogFileDetails(stringBuilder, "ImageFilePath", result.Image.ImageFilePath, result.Image.ImageError);
                LogFileDetails(stringBuilder, "OriginalImageFilePath", result.Image.OriginalImageFilePath, result.Image.OriginalImageError);
                LogFileDetails(stringBuilder, "EditedImageFilePath", result.Image.EditedImageFilePath, result.Image.EditedImageError);
                stringBuilder.AppendLine("CropRect: " + result.Image.CropRect.ToString());
            }

            if (result.Movie != null)
            {
                LogFileDetails(stringBuilder, "MovieFilePath", result.Movie.MovieFilePath, result.Movie.MovieFileError);
            }
            
            Debug.Log(stringBuilder.ToString());
        });
    }

    private static void LogFileDetails(StringBuilder stringBuilder, string fieldName, string fieldFilePath, IIosError error)
    {
        stringBuilder.AppendLine(fieldName + ": " + fieldFilePath);
        if (fieldFilePath != null)
        {
            var fileInfo = new FileInfo(fieldFilePath);
            stringBuilder.AppendLine(fieldName + " Exists: " + fileInfo.Exists);
            stringBuilder.AppendLine(fieldName + " Size: " + fileInfo.Length);
        }

        if (error != null)
        {
            stringBuilder.AppendLine(fieldName + " Error Code: " + error.Code);
            stringBuilder.AppendLine(fieldName + " Error Domain: " + error.Domain);
            stringBuilder.AppendLine(fieldName + " Error Localized Description: " + error.LocalizedDescription);
        }
    }

    public void CleanupPluginFolderWithoutPreview()
    {
        this.CleanupPluginFolder(false);
    }
    
    public void CleanupPluginFolderWithPreview()
    {
        this.CleanupPluginFolder(true);
    }

    private void CleanupPluginFolder(bool preview)
    {
        if (this._iosImagePicker == null)
        {
            return;            
        }

        var cleanupResult = this._iosImagePicker.CleanupPluginFolder(preview);
        if (cleanupResult == null || cleanupResult.DeletionEntries == null)
        {
            return;
        }

        var totalFileSize = 0UL;
        var stringBuilder = new StringBuilder("CLEANUP:\n");
        for (var i = 0; i < cleanupResult.DeletionEntries.Length; i++)
        {
            var deletionEntry = cleanupResult.DeletionEntries[i];
            var allDetails = new[]
            {
                deletionEntry.Path,
                deletionEntry.FileSize + " bytes",
                deletionEntry.IsDirectory ? "Is a Directory" : "Not a Directory",
                deletionEntry.WouldDelete ? "Would Be Deleted" : "",
                deletionEntry.Deleted ? "Deleted" : "",
            };

            totalFileSize += deletionEntry.FileSize;
            
            stringBuilder.AppendLine(string.Join(" ", allDetails));
        }
        
        Debug.Log(stringBuilder.ToString());

        var summaryDetails = new[]
        {
            preview ? "WOULD DELETE" : "DELETED",
            cleanupResult.DeletionEntries.Length + " files",
            totalFileSize + " bytes total",
        };

        Debug.Log(string.Join(" ", summaryDetails));
    }
    
}
