
    using BatiaSuite.Models;
using BatiaSuite.Models.OrdenesTrabajo;
using BatiaSuite.Models.SolicitudCotizacion;
using BatiaSuite.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using Mopups.Services;

    namespace BatiaSuite.ViewModel.Popups {
        public partial class AgregarProductoViewModel : ObservableObject {
            [ObservableProperty]
            private string _tituloPopup = "Agregar Producto";

            [ObservableProperty]
            private string _claveProducto;

            [ObservableProperty]
            private string _nombreProducto;

            [ObservableProperty]
            private string _marcaProducto;

            [ObservableProperty]
            private int _unidadProducto = 0;

            [ObservableProperty]
            private int _cantidadProducto = 1;

            [ObservableProperty]
            private string _fotoPathProducto;

            [ObservableProperty]
            private bool _materialFueraDeInventario;

            [ObservableProperty]
            public int _tipoProducto = 0;

        // Propiedades computadas para mostrar/ocultar contenedores
        public bool EsProductoInterno => TipoProducto == 0;
        public bool EsProductoExterno => TipoProducto == 1;

        private SolicitudCotizacionProductos _productoEditando;
            private Action<SolicitudCotizacionProductos> _onGuardarCallback;

            public void InicializarParaAgregar(Action<SolicitudCotizacionProductos> onGuardar) {
                _onGuardarCallback = onGuardar;
                _productoEditando = null;
                TituloPopup = "Agregar Producto";
                LimpiarCampos();
            }

            public void InicializarParaEditar(SolicitudCotizacionProductos producto, Action<SolicitudCotizacionProductos> onGuardar) {
                _onGuardarCallback = onGuardar;
                _productoEditando = producto;
                TituloPopup = "Editar Producto";

                // Llenar campos con datos del producto
                ClaveProducto = producto.Clave;
                NombreProducto = producto.Nombre;
                MarcaProducto = producto.Marca;
                UnidadProducto = producto.Unidad;
                CantidadProducto = producto.Cantidad;
                FotoPathProducto = producto.FotoPath;
                MaterialFueraDeInventario = producto.MaterialFueraDeInventario;
            }

            [RelayCommand]
            private async Task TomarFoto() {
                try {
                    if(MediaPicker.Default.IsCaptureSupported) {
                        var photo = await MediaPicker.Default.CapturePhotoAsync();

                        if(photo != null) {
                            var localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
                            using(var sourceStream = await photo.OpenReadAsync())
                            using(var localStream = File.OpenWrite(localFilePath)) {
                                await sourceStream.CopyToAsync(localStream);
                            }

                            FotoPathProducto = localFilePath;
                        }
                    } else {
                        await App.Current.MainPage.DisplayAlert("Error", "La cámara no está disponible", "Aceptar");
                    }
                } catch(Exception ex) {
                    await App.Current.MainPage.DisplayAlert("Error", $"Error al tomar foto: {ex.Message}", "Aceptar");
                }
            }

            [RelayCommand]
            private async Task Guardar() {
                if(string.IsNullOrWhiteSpace(ClaveProducto) || string.IsNullOrWhiteSpace(NombreProducto)) {
                    await App.Current.MainPage.DisplayAlert("Error", "Clave y Nombre son requeridos", "Aceptar");
                    return;
                }

                var producto = new SolicitudCotizacionProductos {
                    Clave = ClaveProducto,
                    Nombre = NombreProducto,
                    Marca = MarcaProducto,
                    Unidad = UnidadProducto,
                    Cantidad = CantidadProducto,
                    FotoPath = FotoPathProducto,
                    MaterialFueraDeInventario = MaterialFueraDeInventario
                };

                _onGuardarCallback?.Invoke(producto);
                await MopupService.Instance.PopAsync();
            }

            [RelayCommand]
            private async Task Cancelar() {
                await MopupService.Instance.PopAsync();
            }

            private void LimpiarCampos() {
                ClaveProducto = string.Empty;
                NombreProducto = string.Empty;
                MarcaProducto = string.Empty;
                UnidadProducto = 0;
                CantidadProducto = 1;
                FotoPathProducto = string.Empty;
                MaterialFueraDeInventario = false;
            }

        [RelayCommand]
        private async void SeleccionarProductoSinga() {
            MaterialResponse material = await PopupUtil.GetMaterialAsync(0, 0);

        }
    }
    }