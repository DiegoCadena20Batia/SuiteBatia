using BatiaSuite.ViewModel;

namespace BatiaSuite.Views;

public partial class RegisterDelivery : ContentPage
{
    //private readonly IMediaPicker mediaPicker;
    private string PathFirma;
    RegisterDeliveryViewModel registerDeliveryViewModel;
    public RegisterDelivery(IMediaPicker mediaPicker)
    {
        InitializeComponent();
        //this.mediaPicker = mediaPicker;

        
        registerDeliveryViewModel = new RegisterDeliveryViewModel(drawingView);
        registerDeliveryViewModel.mediaPicker = mediaPicker;
        BindingContext = registerDeliveryViewModel;
    }

    private async void btn_Firmar_Clicked(object sender, EventArgs e)
    {
//        try
//        {
//            using var stream = await DrawView.GetImageStream(1024, 1024);
//            using var memoryStream = new MemoryStream();
//            stream.CopyTo(memoryStream);

//            stream.Position = 0;
//            memoryStream.Position = 0;
//#if WINDOWS
//		////await System.IO.File.WriteAllBytesAsync(
//		////	@"C:$"signature_{Guid.NewGuid()}.png"", memoryStream.ToArray());
//#elif ANDROID
//        var context = Platform.CurrentActivity;

//        if (OperatingSystem.IsAndroidVersionAtLeast(29))
//        {
//            Android.Content.ContentResolver resolver = context.ContentResolver;
//            Android.Content.ContentValues contentValues = new();
//            contentValues.Put(Android.Provider.MediaStore.IMediaColumns.DisplayName, $"signature_{Guid.NewGuid()}.png");
//            contentValues.Put(Android.Provider.MediaStore.IMediaColumns.MimeType, "image/png");
//            contentValues.Put(Android.Provider.MediaStore.IMediaColumns.RelativePath, "DCIM/" + $"signature");
//            Android.Net.Uri imageUri = resolver.Insert(Android.Provider.MediaStore.Images.Media.ExternalContentUri, contentValues);
//            var os = resolver.OpenOutputStream(imageUri);
//            // Obtener el ID de la imagen desde la URI
//            string imageId = imageUri.LastPathSegment;

//            // Crear una nueva URI basada en la ID de la imagen
//            Android.Net.Uri uri = Android.Provider.MediaStore.Images.Media.ExternalContentUri.BuildUpon().AppendPath(imageId).Build();

//            // Consultar la ruta del archivo utilizando la nueva URI
//            string[] projection = { Android.Provider.MediaStore.IMediaColumns.Data };
//            Android.Database.ICursor cursor = resolver.Query(uri, projection, null, null, null);

//            if (cursor != null && cursor.MoveToFirst())
//            {
//                int columnIndex = cursor.GetColumnIndexOrThrow(Android.Provider.MediaStore.IMediaColumns.Data);
//                string filePath = cursor.GetString(columnIndex);

//                // Aquí 'filePath' contendrá la ruta real del archivo guardado
//                PathFirma = filePath;
//                cursor.Close();
//                Android.Graphics.BitmapFactory.Options options = new();
//                options.InJustDecodeBounds = true;
//                var bitmap = Android.Graphics.BitmapFactory.DecodeStream(stream);
//                bitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Png, 100, os);
//                os.Flush();
//                os.Close();
//            }
//            }
//            else
//            {
//                Java.IO.File storagePath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures);
//                string path = System.IO.Path.Combine(storagePath.ToString(), $"signature_{Guid.NewGuid()}.png");
//                System.IO.File.WriteAllBytes(path, memoryStream.ToArray());
//                var mediaScanIntent = new Android.Content.Intent(Android.Content.Intent.ActionMediaScannerScanFile);
//                mediaScanIntent.SetData(Android.Net.Uri.FromFile(new Java.IO.File(path)));
//                context.SendBroadcast(mediaScanIntent);
//            }
//#elif IOS || MACCATALYST
//            //var image = new UIKit.UIImage(Foundation.NSData.FromArray(memoryStream.ToArray()));
//            //image.SaveToPhotosAlbum((image, error) => {
//            //});
//#endif
//            registerDeliveryViewModel.PathFirmaLocal = PathFirma;
//            registerDeliveryViewModel.IsSignature = false;
//        }
//        catch (Exception ex)
//        {
//            if(ex.Data.Count == 0)
//                await DisplayAlert("Error", "Por favor ingrese su firma", "Ok");
//            else
//                await DisplayAlert("Error",ex.Message,"Ok");
//        }

    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        //DrawView.Clear();
        registerDeliveryViewModel.IsSignature = true;
    }
}