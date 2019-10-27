using System;
using System.Text;
using ImagePicker.Ios;
using UnityEngine;
using UnityEngine.UI;

public class TestMenu : MonoBehaviour
{
    [SerializeField]
    private Text sourceTypeLabel;
    
    [SerializeField]
    private Text allowsEditingLabel;
    
    [SerializeField]
    private Text videoQualityLabel;
    
    [SerializeField]
    private Text videoMaximumDurationLabel;
    
    [SerializeField]
    private Text cameraDeviceLabel;
    
    [SerializeField]
    private Text cameraCaptureModeLabel;
    
    [SerializeField]
    private Text cameraFlashModeLabel;

    private IIosImagePicker _iosImagePicker;

    private void Start()
    {
        var iosImagePicker = new IosImagePicker();
        
        var stringBuilder = new StringBuilder();
        
        stringBuilder.AppendLine("========================================================");
        stringBuilder.AppendLine("imagePicker.IsSourceTypeAvailable");
        stringBuilder.AppendLine("SavedPhotosAlbum " + iosImagePicker.IsSourceTypeAvailable(IosImagePickerSourceType.SavedPhotosAlbum));
        stringBuilder.AppendLine("Camera " + iosImagePicker.IsSourceTypeAvailable(IosImagePickerSourceType.Camera));
        stringBuilder.AppendLine("PhotoLibrary " + iosImagePicker.IsSourceTypeAvailable(IosImagePickerSourceType.PhotoLibrary));
        stringBuilder.AppendLine("========================================================");
        
        stringBuilder.AppendLine("========================================================");
        stringBuilder.AppendLine("imagePicker.IsCameraDeviceAvailable");
        stringBuilder.AppendLine("Front " + iosImagePicker.IsCameraDeviceAvailable(IosImagePickerCameraDevice.Front));
        stringBuilder.AppendLine("Rear " + iosImagePicker.IsCameraDeviceAvailable(IosImagePickerCameraDevice.Rear));
        stringBuilder.AppendLine("========================================================");
        
        stringBuilder.AppendLine("========================================================");
        stringBuilder.AppendLine("imagePicker.AvailableMediaTypesForSourceType");
        stringBuilder.AppendLine("SavedPhotosAlbum " + string.Join("[]", iosImagePicker.AvailableMediaTypesForSourceType(IosImagePickerSourceType.SavedPhotosAlbum)));
        stringBuilder.AppendLine("Camera " + string.Join("[]", iosImagePicker.AvailableMediaTypesForSourceType(IosImagePickerSourceType.Camera)));
        stringBuilder.AppendLine("PhotoLibrary " + string.Join("[]", iosImagePicker.AvailableMediaTypesForSourceType(IosImagePickerSourceType.PhotoLibrary)));
        stringBuilder.AppendLine("========================================================");
        
        stringBuilder.AppendLine("========================================================");
        stringBuilder.AppendLine("imagePicker.AvailableCaptureModesForCameraDevice");
        stringBuilder.AppendLine("Front " + string.Join("[]", iosImagePicker.AvailableCaptureModesForCameraDevice(IosImagePickerCameraDevice.Front)));
        stringBuilder.AppendLine("Rear " + string.Join("[]", iosImagePicker.AvailableCaptureModesForCameraDevice(IosImagePickerCameraDevice.Rear)));
        stringBuilder.AppendLine("========================================================");
        
        stringBuilder.AppendLine("========================================================");
        stringBuilder.AppendLine("imagePicker.IsFlashAvailableForCameraDevice");
        stringBuilder.AppendLine("Front " + iosImagePicker.IsFlashAvailableForCameraDevice(IosImagePickerCameraDevice.Front));
        stringBuilder.AppendLine("Rear " + iosImagePicker.IsFlashAvailableForCameraDevice(IosImagePickerCameraDevice.Rear));
        stringBuilder.AppendLine("========================================================");
        
        stringBuilder.AppendLine("========================================================");
        stringBuilder.AppendLine("imagePicker media types");
        stringBuilder.AppendLine("MediaTypeImage " + iosImagePicker.MediaTypeImage);
        stringBuilder.AppendLine("MediaTypeMovie " + iosImagePicker.MediaTypeMovie);
        stringBuilder.AppendLine("========================================================");
        
        Debug.Log(stringBuilder.ToString());

        this._iosImagePicker = iosImagePicker;
        this.UpdateTexts();
    }

    public void Present()
    {
        this._iosImagePicker.Present();
    }

    public void ChangeSourceType()
    {
        var sourceType = this._iosImagePicker.SourceType;
        
        switch (sourceType)
        {
            case IosImagePickerSourceType.PhotoLibrary: sourceType = IosImagePickerSourceType.Camera; break;
            case IosImagePickerSourceType.Camera: sourceType = IosImagePickerSourceType.SavedPhotosAlbum; break;
            case IosImagePickerSourceType.SavedPhotosAlbum: sourceType = IosImagePickerSourceType.PhotoLibrary; break;
        }

        this._iosImagePicker.SourceType = sourceType;
        UpdateTexts();
    }
    
    public void ChangeAllowsEditing()
    {
        this._iosImagePicker.AllowsEditing = !this._iosImagePicker.AllowsEditing;
        UpdateTexts();
    }
    
    public void ChangeVideoQuality()
    {
        var videoQuality = this._iosImagePicker.VideoQuality;
        switch (videoQuality)
        {
            case IosImagePickerVideoQualityType.High: videoQuality = IosImagePickerVideoQualityType.Medium; break;
            case IosImagePickerVideoQualityType.Medium: videoQuality = IosImagePickerVideoQualityType.Low; break;
            case IosImagePickerVideoQualityType.Low: videoQuality = IosImagePickerVideoQualityType.VGA640x480; break;
            case IosImagePickerVideoQualityType.VGA640x480: videoQuality = IosImagePickerVideoQualityType.IFrame1280x720; break;
            case IosImagePickerVideoQualityType.IFrame1280x720: videoQuality = IosImagePickerVideoQualityType.IFrame960x540; break;
            case IosImagePickerVideoQualityType.IFrame960x540: videoQuality = IosImagePickerVideoQualityType.High; break;
        }

        this._iosImagePicker.VideoQuality = videoQuality;
        UpdateTexts();
    }

    public void ChangeVideoMaxDuration()
    {
        var totalSeconds = this._iosImagePicker.VideoMaximumDuration.TotalSeconds;
        if (totalSeconds >= 600.0)
        {
            this._iosImagePicker.VideoMaximumDuration = TimeSpan.FromSeconds(200.0);   
        }
        else if (totalSeconds >= 200.0)
        {
            this._iosImagePicker.VideoMaximumDuration = TimeSpan.FromSeconds(30.0);   
        }
        else
        {
            this._iosImagePicker.VideoMaximumDuration = TimeSpan.FromSeconds(600.0); 
        }
        
        UpdateTexts();
    }

    public void ChangeCameraDevice()
    {
        var cameraDevice = this._iosImagePicker.CameraDevice;
        switch (cameraDevice)
        {
            case IosImagePickerCameraDevice.Rear: cameraDevice = IosImagePickerCameraDevice.Front; break;
            case IosImagePickerCameraDevice.Front: cameraDevice = IosImagePickerCameraDevice.Rear; break;
        }
        
        this._iosImagePicker.CameraDevice = cameraDevice;
        UpdateTexts();
    }

    public void ChangeCameraCaptureMode()
    {
        var cameraCaptureMode = this._iosImagePicker.CameraCaptureMode;
        var mediaTypes = this._iosImagePicker.MediaTypes;
        switch (cameraCaptureMode)
        {
            case IosImagePickerVideoCaptureMode.Photo:
                cameraCaptureMode = IosImagePickerVideoCaptureMode.Video;
                mediaTypes = new[] { this._iosImagePicker.MediaTypeMovie };
                break;
            
            case IosImagePickerVideoCaptureMode.Video:
                cameraCaptureMode = IosImagePickerVideoCaptureMode.Photo;
                mediaTypes = new[] { this._iosImagePicker.MediaTypeImage };
                break;
        }
        
        this._iosImagePicker.CameraCaptureMode = cameraCaptureMode;
        this._iosImagePicker.MediaTypes = mediaTypes;
        UpdateTexts();
    }
    
    public void ChangeCameraFlashMode()
    {
        var cameraFlashMode = this._iosImagePicker.CameraFlashMode;
        switch (cameraFlashMode)
        {
            case IosImagePickerCameraFlashMode.Auto: cameraFlashMode = IosImagePickerCameraFlashMode.Off; break;
            case IosImagePickerCameraFlashMode.Off: cameraFlashMode = IosImagePickerCameraFlashMode.On; break;
            case IosImagePickerCameraFlashMode.On: cameraFlashMode = IosImagePickerCameraFlashMode.Auto; break;
        }

        this._iosImagePicker.CameraFlashMode = cameraFlashMode;
        UpdateTexts();
    }

    private void UpdateTexts()
    {
        this.sourceTypeLabel.text = this._iosImagePicker.SourceType.ToString();
        this.allowsEditingLabel.text = this._iosImagePicker.AllowsEditing ? "Allows Editing" : "Editing not allowed";
        this.videoQualityLabel.text = this._iosImagePicker.VideoQuality.ToString();
        this.videoMaximumDurationLabel.text = this._iosImagePicker.VideoMaximumDuration.TotalSeconds + " seconds";
        this.cameraDeviceLabel.text = this._iosImagePicker.CameraDevice.ToString();
        this.cameraCaptureModeLabel.text = this._iosImagePicker.CameraCaptureMode.ToString();
        this.cameraFlashModeLabel.text = this._iosImagePicker.CameraFlashMode.ToString();
    }
}
