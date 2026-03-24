using BatiaSuite.ViewModel;

namespace BatiaSuite.Views;

public partial class RegistrosCorrctivosM : ContentPage
{
    RegistrosCorrctivosMViewModel registro = new RegistrosCorrctivosMViewModel();
    //RegistrosCorrctivosMViewModel registrosCorrctivosMViewModel = new RegistrosCorrctivosMViewModel();

    private string PathFirma;
    public RegistrosCorrctivosM()
	{

		InitializeComponent();
		BindingContext = registro;
     
        //BindingContext = registrosCorrctivosMViewModel;
    }

    private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        registro.IsEncuesta();
    }

    private async void btn_Firmar_Clicked(object sender, EventArgs e) 
    {
   
        try
        {
            using var stream = await DrawView.GetImageStream(1024, 1024);
            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);

            stream.Position = 0;
            memoryStream.Position = 0;
#if WINDOWS
            ////await System.IO.File.WriteAllBytesAsync(
            ////	@"C:$"signature_{Guid.NewGuid()}.png"", memoryStream.ToArray());
#elif ANDROID
        var context = Platform.CurrentActivity;

        if (OperatingSystem.IsAndroidVersionAtLeast(29))
        {
            Android.Content.ContentResolver resolver = context.ContentResolver;
            Android.Content.ContentValues contentValues = new();
            contentValues.Put(Android.Provider.MediaStore.IMediaColumns.DisplayName, $"signature_{Guid.NewGuid()}.png");
            contentValues.Put(Android.Provider.MediaStore.IMediaColumns.MimeType, "image/png");
            contentValues.Put(Android.Provider.MediaStore.IMediaColumns.RelativePath, "DCIM/" + $"signature");
            Android.Net.Uri imageUri = resolver.Insert(Android.Provider.MediaStore.Images.Media.ExternalContentUri, contentValues);
            var os = resolver.OpenOutputStream(imageUri);
            // Obtener el ID de la imagen desde la URI
            string imageId = imageUri.LastPathSegment;

            // Crear una nueva URI basada en la ID de la imagen
            Android.Net.Uri uri = Android.Provider.MediaStore.Images.Media.ExternalContentUri.BuildUpon().AppendPath(imageId).Build();

            // Consultar la ruta del archivo utilizando la nueva URI
            string[] projection = { Android.Provider.MediaStore.IMediaColumns.Data };
            Android.Database.ICursor cursor = resolver.Query(uri, projection, null, null, null);

            if (cursor != null && cursor.MoveToFirst())
            {
                int columnIndex = cursor.GetColumnIndexOrThrow(Android.Provider.MediaStore.IMediaColumns.Data);
                string filePath = cursor.GetString(columnIndex);

                // Aquí 'filePath' contendrá la ruta real del archivo guardado
                PathFirma = filePath;
                cursor.Close();
                Android.Graphics.BitmapFactory.Options options = new();
                options.InJustDecodeBounds = true;
                var bitmap = Android.Graphics.BitmapFactory.DecodeStream(stream);
                bitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Png, 100, os);
                os.Flush();
                os.Close();
            }
            }
            else
            {
                Java.IO.File storagePath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures);
                string path = System.IO.Path.Combine(storagePath.ToString(), $"signature_{Guid.NewGuid()}.png");
                System.IO.File.WriteAllBytes(path, memoryStream.ToArray());
                var mediaScanIntent = new Android.Content.Intent(Android.Content.Intent.ActionMediaScannerScanFile);
                mediaScanIntent.SetData(Android.Net.Uri.FromFile(new Java.IO.File(path)));
                context.SendBroadcast(mediaScanIntent);
            }
#elif IOS || MACCATALYST
            //var image = new UIKit.UIImage(Foundation.NSData.FromArray(memoryStream.ToArray()));
            //image.SaveToPhotosAlbum((image, error) =>
            //{
            //});
#endif
            registro.PathFirmaLocal = PathFirma;
            registro.IsSignature = false;
        }
        catch (Exception ex)
        {
            if (ex.Data.Count == 0)
                await DisplayAlert("Error", "Por favor ingrese su firma", "Ok");
            else
                await DisplayAlert("Error", ex.Message, "Ok");
        }
       
    }

    private void btn_Clear_Clicked(object sender, EventArgs e)
    {
        DrawView.Clear();
        registro.IsSignature = true;
    }

    private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        RadioButton radio = sender as RadioButton;
        if (radio.IsChecked)
        {
            // Convierte el valor del radio button a un entero
            int.TryParse(radio.Value.ToString(), out int intValue);

            // Ahora puedes usar 'intValue' como un número entero
            registro.SelectionRadio = intValue;
        }
    }

    private void RadioButton_CheckedChanged_1(object sender, CheckedChangedEventArgs e)
    {
        RadioButton radio = sender as RadioButton;
        if (radio.IsChecked)
        {
            // Convierte el valor del radio button a un entero
            int.TryParse(radio.Value.ToString(), out int intValue);

            // Ahora puedes usar 'intValue' como un número entero
            registro.SelectionRadio1 = intValue;
        }
    }

    private void RadioButton_CheckedChanged_2(object sender, CheckedChangedEventArgs e)
    {
        RadioButton radio = sender as RadioButton;
        if (radio.IsChecked)
        {
            // Convierte el valor del radio button a un entero
            int.TryParse(radio.Value.ToString(), out int intValue);

            // Ahora puedes usar 'intValue' como un número entero
            registro.SelectionRadio2 = intValue;
        }
    }

    private void RadioButton_CheckedChanged_3(object sender, CheckedChangedEventArgs e)
    {
        RadioButton radio = sender as RadioButton;
        if (radio.IsChecked)
        {
            // Convierte el valor del radio button a un entero
            int.TryParse(radio.Value.ToString(), out int intValue);

            // Ahora puedes usar 'intValue' como un número entero
            registro.SelectionRadio3 = intValue;
        }
    }

    private void RadioButton_CheckedChanged_4(object sender, CheckedChangedEventArgs e)
    {
        RadioButton radio = sender as RadioButton;
        if (radio.IsChecked)
        {
            // Convierte el valor del radio button a un entero
            int.TryParse(radio.Value.ToString(), out int intValue);

            // Ahora puedes usar 'intValue' como un número entero
            registro.SelectionRadio4 = intValue;
        }
    }
}