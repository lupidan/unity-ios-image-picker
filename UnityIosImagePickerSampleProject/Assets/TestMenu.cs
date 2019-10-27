using System.Text;
using ImagePicker.Ios;
using UnityEngine;

public class TestMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var imagePicker = new IosImagePicker();
        var stringBuilder = new StringBuilder();
        
        stringBuilder.AppendLine("========================================================");
        stringBuilder.AppendLine("imagePicker.IsSourceTypeAvailable");
        stringBuilder.AppendLine("SavedPhotosAlbum " + imagePicker.IsSourceTypeAvailable(IosImagePickerSourceType.SavedPhotosAlbum));
        stringBuilder.AppendLine("Camera " + imagePicker.IsSourceTypeAvailable(IosImagePickerSourceType.Camera));
        stringBuilder.AppendLine("PhotoLibrary " + imagePicker.IsSourceTypeAvailable(IosImagePickerSourceType.PhotoLibrary));
        stringBuilder.AppendLine("========================================================");
        
        stringBuilder.AppendLine("========================================================");
        stringBuilder.AppendLine("imagePicker.IsCameraDeviceAvailable");
        stringBuilder.AppendLine("Front " + imagePicker.IsCameraDeviceAvailable(IosImagePickerCameraDevice.Front));
        stringBuilder.AppendLine("Rear " + imagePicker.IsCameraDeviceAvailable(IosImagePickerCameraDevice.Rear));
        stringBuilder.AppendLine("========================================================");
        
        stringBuilder.AppendLine("========================================================");
        stringBuilder.AppendLine("imagePicker.AvailableMediaTypesForSourceType");
        stringBuilder.AppendLine("SavedPhotosAlbum " + string.Join("[]", imagePicker.AvailableMediaTypesForSourceType(IosImagePickerSourceType.SavedPhotosAlbum)));
        stringBuilder.AppendLine("Camera " + string.Join("[]", imagePicker.AvailableMediaTypesForSourceType(IosImagePickerSourceType.Camera)));
        stringBuilder.AppendLine("PhotoLibrary " + string.Join("[]", imagePicker.AvailableMediaTypesForSourceType(IosImagePickerSourceType.PhotoLibrary)));
        stringBuilder.AppendLine("========================================================");
        
        stringBuilder.AppendLine("========================================================");
        stringBuilder.AppendLine("imagePicker.AvailableCaptureModesForCameraDevice");
        stringBuilder.AppendLine("Front " + string.Join("[]", imagePicker.AvailableCaptureModesForCameraDevice(IosImagePickerCameraDevice.Front)));
        stringBuilder.AppendLine("Rear " + string.Join("[]", imagePicker.AvailableCaptureModesForCameraDevice(IosImagePickerCameraDevice.Rear)));
        stringBuilder.AppendLine("========================================================");
        
        stringBuilder.AppendLine("========================================================");
        stringBuilder.AppendLine("imagePicker.IsFlashAvailableForCameraDevice");
        stringBuilder.AppendLine("Front " + imagePicker.IsFlashAvailableForCameraDevice(IosImagePickerCameraDevice.Front));
        stringBuilder.AppendLine("Rear " + imagePicker.IsFlashAvailableForCameraDevice(IosImagePickerCameraDevice.Rear));
        stringBuilder.AppendLine("========================================================");
        
        Debug.Log(stringBuilder.ToString());
    }

    public void Present()
    {
        var imagePicker = new IosImagePicker();
        imagePicker.Present();
    }
}
