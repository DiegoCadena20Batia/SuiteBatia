#if __IOS__
using System.Drawing;
using UIKit;
using CoreGraphics;
#endif

#if __ANDROID__
using Android.Graphics;
using System.IO;
using AndroidX.ExifInterface.Media; 
#endif

namespace BatiaSuite.Utils;

public static class ImageResizerHelper {

    static ImageResizerHelper() {
    }

    public static async Task<byte[]> ResizeImage(byte[] imageData, int width, int height, bool rotate = false) {

        //rotate = false;
#if __IOS__
        return ResizeImageIOS(imageData, width, height);
#endif
#if __ANDROID__
        return ResizeImageAndroid(imageData, width, height, rotate);
#else
        return null;
#endif
    }


#if __IOS__
    public static byte[] ResizeImageIOS(byte[] imageData, int maxWidth, int maxHeight) {
        //UIImage originalImage = ImageFromByteArray(imageData);
        //UIImageOrientation orientation = originalImage.Orientation;

        ////create a 24bit RGB image
        //using(CGBitmapContext context = new CGBitmapContext(IntPtr.Zero,
        //                                     width, height, 8,
        //                                     4 * width, CGColorSpace.CreateDeviceRGB(),
        //                                     CGImageAlphaInfo.PremultipliedFirst)) {

        //    RectangleF imageRect = new RectangleF(0, 0, width, height);

        //    // draw the image
        //    context.DrawImage(imageRect, originalImage.CGImage);
        //    //rotated 90° counterclockwise from the orientation of its original pixel data.
        //    UIKit.UIImage resizedImage = UIKit.UIImage.FromImage(context.ToImage(), 0, UIImageOrientation.Left);

        //    // save the image as a jpeg
        //    return resizedImage.AsJPEG().ToArray();
        //}
        UIImage originalImage = ImageFromByteArray(imageData);
        if (originalImage == null) return null;

        nfloat originalWidth = originalImage.Size.Width;
        nfloat originalHeight = originalImage.Size.Height;

        float ratioX = (float)((nfloat)maxWidth / originalWidth);
        float ratioY = (float)((nfloat)maxHeight / originalHeight);
        float ratio = Math.Min(ratioX, ratioY);

        int newWidth = (int)(originalWidth * ratio);
        int newHeight = (int)(originalHeight * ratio);

        UIGraphics.BeginImageContext(new CoreGraphics.CGSize(newWidth, newHeight));
        originalImage.Draw(new CoreGraphics.CGRect(0, 0, newWidth, newHeight));
        UIImage resizedImage = UIGraphics.GetImageFromCurrentImageContext();
        UIGraphics.EndImageContext();

        return resizedImage.AsJPEG().ToArray();
    

    }

    private static UIImage ImageFromByteArray(byte[] data) {
        if(data == null) {
            return null;
        }

        UIKit.UIImage image;
        try {
            image = new UIKit.UIImage(Foundation.NSData.FromArray(data));
        } catch(Exception e) {
            Console.WriteLine("Image load failed: " + e.Message);
            return null;
        }
        return image;
    }
#endif

#if __ANDROID__

    //public static byte[] ResizeImageAndroid(byte[] imageData, int width, int height, bool rotate) {

    //    Bitmap originalImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
    //    Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, width, height, false);       

    //    if(DeviceInfo.Current.Idiom == DeviceIdiom.Tablet || !rotate) {
    //        using(MemoryStream ms = new MemoryStream()) {
    //            resizedImage.Compress(Bitmap.CompressFormat.Jpeg, 100, ms);
    //            return ms.ToArray();
    //        }
    //    }

    //    Matrix matrix = new Matrix();                                      
    //    matrix.SetRotate(90);
    //    Bitmap RotatedImage = Bitmap.CreateBitmap(resizedImage, 0, 0, resizedImage.Width, resizedImage.Height, matrix, true);
    //    using(MemoryStream ms = new MemoryStream()) {
    //        RotatedImage.Compress(Bitmap.CompressFormat.Jpeg, 100, ms);
    //        return ms.ToArray();
    //    }
    //}

    public static byte[] ResizeImageAndroid(byte[] imageData, int maxWidth, int maxHeight, bool applyExifRotation = true)
{
    if (imageData == null || imageData.Length == 0)
        return imageData;

    // 1) Decodificar bitmap original
    Bitmap original = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
    if (original == null)
        return null;

    try
    {
        // 2) Leer orientación EXIF desde el byte[] (si se solicita)
        int rotationDegrees = 0;
        if (applyExifRotation)
        {
            using (var ms = new MemoryStream(imageData))
            {
                var exif = new ExifInterface(ms);
                int orient = exif.GetAttributeInt(ExifInterface.TagOrientation, (int)ExifInterface.OrientationNormal);
switch (orient)
{
    case (int)ExifInterface.OrientationRotate90:
        rotationDegrees = 90;
        break;
    case (int)ExifInterface.OrientationRotate180:
        rotationDegrees = 180;
        break;
    case (int)ExifInterface.OrientationRotate270:
        rotationDegrees = 270;
        break;
}
            }
        }

        // 3) Si necesita rotación, aplicarla al bitmap original (antes del resize)
        Bitmap oriented = original;
        if (rotationDegrees != 0)
        {
            using (var matrix = new Matrix())
            {
                matrix.PostRotate(rotationDegrees);
                oriented = Bitmap.CreateBitmap(original, 0, 0, original.Width, original.Height, matrix, true);
            }
            // liberar el original (ya no lo necesitamos)
            original.Recycle();
            original.Dispose();
        }

        // 4) Calcular escala manteniendo proporción (usar las dimensiones ya orientadas)
        int oW = oriented.Width;
        int oH = oriented.Height;

        float ratioX = (float)maxWidth / oW;
        float ratioY = (float)maxHeight / oH;
        float ratio = Math.Min(ratioX, ratioY);

        if (ratio > 1f) ratio = 1f; // no escalar hacia arriba

        int newW = (int)(oW * ratio);
        int newH = (int)(oH * ratio);

        // 5) Crear bitmap redimensionado
        Bitmap resized = Bitmap.CreateScaledBitmap(oriented, newW, newH, true);

        // Si 'oriented' y 'resized' son distintos, liberar 'oriented'
        if (!object.ReferenceEquals(oriented, resized))
        {
            oriented.Recycle();
            oriented.Dispose();
        }

        // 6) Comprimir a JPEG y devolver byte[]
        using (var outStream = new MemoryStream())
        {
            resized.Compress(Bitmap.CompressFormat.Jpeg, 100, outStream);
            resized.Recycle();
            resized.Dispose();
            return outStream.ToArray();
        }
    }
    finally
    {
        // seguridad: liberar si algo quedó
        try { original?.Recycle(); original?.Dispose(); } catch { }
    }
}


#endif

}
