namespace IosImagePicker.IOS.NativeMessages
{
    public static class NativeSerializationTools
    {
        internal static void FixSerializationForString(ref string originalString)
        {
            if (string.IsNullOrEmpty(originalString))
                originalString = null;
        }
        
        internal static void FixSerializationForObject<T>(ref T originalObject, bool containsObject)
        {
            if (!containsObject)
                originalObject = default(T);
        }
    }
}
