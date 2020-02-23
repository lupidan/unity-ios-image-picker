using System;
using System.Collections.Generic;
using DefaultNamespace;
using IosImagePicker;
using IosImagePicker.Enums;
using UnityEngine;
using UnityEngine.UI;
using IosImagePicker;
using IosImagePicker.IOS.NativeMessages;
using System.IO;
using System.Text;
using IosImagePicker = IosImagePicker.IosImagePicker;

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
        var payloadDeserializer = new PayloadDeserializer();
        var iosImagePicker = new global::IosImagePicker.IosImagePicker(payloadDeserializer);
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
        
        var dropdownController = new DropdownController<IosImagePickerSourceType>(
            allSourceTypes,
            rowGameObject,
            dropdown,
            () => iosImagePicker.SourceType,
            sourceType => iosImagePicker.SourceType = sourceType,
            sourceType => iosImagePicker.IsSourceTypeAvailable(sourceType));
        
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

        var dropdownController = new DropdownController<IosImagePickerCameraDevice>(
            allCameraDevices,
            rowGameObject,
            dropdown,
            () => iosImagePicker.CameraDevice,
            cameraDevice => iosImagePicker.CameraDevice = cameraDevice,
            cameraDevice => iosImagePicker.IsCameraDeviceAvailable(cameraDevice));
        
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

        var dropdownController = new DropdownController<IosImagePickerCameraCaptureMode>(
            allCaptureModes,
            rowGameObject,
            dropdown,
            () => iosImagePicker.CameraCaptureMode,
            captureMode => iosImagePicker.CameraCaptureMode = captureMode,
            captureMode =>
            {
                var currentCameraDevice = iosImagePicker.CameraDevice;
                var availableCaptureModes = iosImagePicker.AvailableCaptureModesForCameraDevice(currentCameraDevice);
                return availableCaptureModes != null && Array.IndexOf(availableCaptureModes, captureMode) > -1;
            });
        
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

        var dropdownController = new DropdownController<IosImagePickerCameraFlashMode>(
            allFlashModes,
            rowGameObject,
            dropdown,
            () => iosImagePicker.CameraFlashMode,
            flashMode => iosImagePicker.CameraFlashMode = flashMode,
            flashMode =>
            {
                var currentCameraDevice = iosImagePicker.CameraDevice;
                return flashMode != IosImagePickerCameraFlashMode.On || iosImagePicker.IsFlashAvailableForCameraDevice(currentCameraDevice);
            });

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

        var dropdownController = new DropdownController<IosImagePickerVideoQualityType>(
            allQualityTypes,
            rowGameObject,
            dropdown,
            () => iosImagePicker.VideoQuality,
            videoQuality => iosImagePicker.VideoQuality = videoQuality,
            _ =>
            {
                var currentMediaTypes = iosImagePicker.MediaTypes;
                return currentMediaTypes != null && Array.IndexOf(currentMediaTypes, iosImagePicker.MediaTypeMovie) > -1;
            });
        
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
        
        var dropdownController = new DropdownController<TimeSpan>(
            allMaxDurations,
            rowGameObject,
            dropdown,
            () => iosImagePicker.VideoMaximumDuration,
            maxDuration => iosImagePicker.VideoMaximumDuration = maxDuration,
            _ =>
            {
                var currentMediaTypes = iosImagePicker.MediaTypes;
                return currentMediaTypes != null && Array.IndexOf(currentMediaTypes, iosImagePicker.MediaTypeMovie) > -1;
            });
        
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

        var toggleGroupController = new ToggleGroupController<string>(
            values,
            labels,
            toggles,
            () => iosImagePicker.MediaTypes,
            mediaTypes => iosImagePicker.MediaTypes = mediaTypes,
            mediaType =>
            {
                var availableMediaTypes = iosImagePicker.AvailableMediaTypesForSourceType(iosImagePicker.SourceType);
                return availableMediaTypes != null && Array.IndexOf(availableMediaTypes, mediaType) > -1;
            });
        
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
                stringBuilder.AppendLine("ImageFileUrl: " + result.Image.ImageFileUrl);
                if (result.Image.ImageFileUrl != null)
                {
                    var uri = new Uri(result.Image.ImageFileUrl);
                    var fileInfo = new FileInfo(uri.AbsolutePath);
                    stringBuilder.AppendLine("ImageFileUrl Exists: " + fileInfo.Exists);
                    stringBuilder.AppendLine("ImageFileUrl Size: " + fileInfo.Length);
                }
                
                stringBuilder.AppendLine("OriginalImageFileUrl: " + result.Image.OriginalImageFileUrl);
                if (result.Image.OriginalImageFileUrl != null)
                {
                    var uri = new Uri(result.Image.OriginalImageFileUrl);
                    var fileInfo = new FileInfo(uri.AbsolutePath);
                    stringBuilder.AppendLine("ImageFileUrl Exists: " + fileInfo.Exists);
                    stringBuilder.AppendLine("ImageFileUrl Size: " + fileInfo.Length);
                }
                
                stringBuilder.AppendLine("EditedImageFileUrl: " + result.Image.EditedImageFileUrl);
                if (result.Image.EditedImageFileUrl != null)
                {
                    var uri = new Uri(result.Image.EditedImageFileUrl);
                    var fileInfo = new FileInfo(uri.AbsolutePath);
                    stringBuilder.AppendLine("EditedImageFileUrl Exists: " + fileInfo.Exists);
                    stringBuilder.AppendLine("EditedImageFileUrl Size: " + fileInfo.Length);
                }
                
                stringBuilder.AppendLine("CropRect: " + result.Image.CropRect.ToString());
            }

            if (result.Movie != null)
            {
                stringBuilder.AppendLine("MovieFileUrl: " + result.Movie.MovieFileUrl);
                if (result.Movie.MovieFileUrl != null)
                {
                    var uri = new Uri(result.Movie.MovieFileUrl);
                    var fileInfo = new FileInfo(uri.AbsolutePath);
                    stringBuilder.AppendLine("MovieFileUrl Exists: " + fileInfo.Exists);
                    stringBuilder.AppendLine("MovieFileUrl Size: " + fileInfo.Length);
                }
            }
            
            Debug.Log(stringBuilder.ToString());
        });
    }
}
