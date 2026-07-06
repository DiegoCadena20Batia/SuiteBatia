using BatiaSuite.Data;
using BatiaSuite.Models.EntidadesLocal.RutasEntregas;
using BatiaSuite.Utils;
using BatiaSuite.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel; // Necesario para Launcher en .NET MAUI

namespace BatiaSuite.ViewModel.RutasEntregas {
    public partial class TiposListadoViewModel : ViewModelBase, IQueryAttributable {
        private ObservableCollection<string> _listTipos;
        public ObservableCollection<string> ListTipos {
            get => _listTipos;
            set { _listTipos = value; OnPropertyChanged(); }
        }

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _textLoading;

        [ObservableProperty]
        private string _nombreSucursal;
        [ObservableProperty]
        private string _folioListado;

        // Variables locales para retener las coordenadas de la sucursal
        private string _latitud;
        private string _longitud;

        private readonly LocalDbContext _dbContext;

        public TiposListadoViewModel() {
            _dbContext = new LocalDbContext();
            ListTipos = new ObservableCollection<string>();
        }

        /// <summary>
        /// Se ejecuta al entrar a la pantalla
        /// </summary>
        public void ApplyQueryAttributes(IDictionary<string, object> query) {
            NombreSucursal = UserSession.InmuebleNameTracking;

            _ = ObtenerTiposDeListado();
        }

        /// <summary>
        /// Extrae los tipos únicos (Iguala, Adicionales, etc.) de la base de datos local desnormalizada
        /// </summary>
        public async Task ObtenerTiposDeListado() {
            try {
                IsLoading = true;
                TextLoading = "Consultando...";
                await Task.Delay(300);

                var registrosSucursal = await _dbContext.ObtenerListaLocalAsync<RutasInmuebles>(r =>
                    r.IdInmueble == UserSession.IdInmuebleTracking
                );

                if(registrosSucursal != null && registrosSucursal.Count > 0) {
                    var tiposUnicos = registrosSucursal
                        .Where(r => !string.IsNullOrWhiteSpace(r.Tipo))
                        .GroupBy(r => r.Tipo)
                        .Select(g => g.Key)
                        .ToList();

                    ListTipos = new ObservableCollection<string>(tiposUnicos);

                    var primerRegistro = registrosSucursal.FirstOrDefault();
                    FolioListado = primerRegistro?.IdListado.ToString() ?? "N/A";

                    // Resguardamos las coordenadas de este inmueble específico
                    _latitud = primerRegistro?.Latitud;
                    _longitud = primerRegistro?.Longitud;
                } else {
                    ListTipos = new ObservableCollection<string>();
                }
            } catch(Exception ex) {
                Console.WriteLine($"Error al agrupar tipos de listado: {ex.Message}");
                ListTipos = new ObservableCollection<string>();
            } finally {
                IsLoading = false;
                TextLoading = "";
            }
        }

        /// <summary>
        /// Comando que se ejecuta cuando el operador selecciona una categoría de la lista
        /// </summary>
        [RelayCommand]
        private async Task TipoSelec(string tipoSeleccionado) {
            if(string.IsNullOrEmpty(tipoSeleccionado)) return;

            try {
                IsLoading = true;
                TextLoading = $"Abriendo {tipoSeleccionado}...";
                await Task.Delay(300);

                UserSession.TipoListadoTracking = tipoSeleccionado;

                // Avanzamos hacia la pantalla final de detalle de materiales
                await Shell.Current.GoToAsync(nameof(ListadoMateriales), true);
            } catch(Exception ex) {
                await Shell.Current.DisplayAlert("Error", $"No se pudo abrir la categoría: {ex.Message}", "Ok");
            } finally {
                IsLoading = false;
                TextLoading = "";
            }
        }

        /// <summary>
        /// Lanza de forma nativa la aplicación de Google Maps hacia la sucursal actual
        /// </summary>
        [RelayCommand]
        private async Task AbrirGoogleMaps() {
            if(string.IsNullOrEmpty(_latitud) || string.IsNullOrEmpty(_longitud)) {
                await Shell.Current.DisplayAlert("GPS", "No se encontraron coordenadas válidas para esta sucursal.", "Ok");
                return;
            }

            try {
                string url = $"http://maps.google.com/?daddr={_latitud},{_longitud}";
                if(await Launcher.CanOpenAsync(url)) {
                    await Launcher.OpenAsync(url);
                } else {
                    await Shell.Current.DisplayAlert("Google Maps", "No se pudo abrir Google Maps en este dispositivo.", "Ok");
                }
            } catch(Exception ex) {
                await Shell.Current.DisplayAlert("Error", $"Error al intentar abrir el mapa: {ex.Message}", "Ok");
            }
        }

        /// <summary>
        /// Lanza de forma nativa la aplicación de Waze hacia la sucursal actual (con fallback web)
        /// </summary>
        [RelayCommand]
        private async Task AbrirWaze() {
            if(string.IsNullOrEmpty(_latitud) || string.IsNullOrEmpty(_longitud)) {
                await Shell.Current.DisplayAlert("GPS", "No se encontraron coordenadas válidas para esta sucursal.", "Ok");
                return;
            }

            try {
                string wazeUrl = $"waze://?ll={_latitud},{_longitud}&navigate=yes";
                string webUrl = $"https://waze.com/ul?ll={_latitud},{_longitud}&navigate=yes";

                if(await Launcher.CanOpenAsync(wazeUrl)) {
                    await Launcher.OpenAsync(wazeUrl);
                } else {
                    // Si no tiene la app instalada, lo abre en el navegador web
                    await Launcher.OpenAsync(webUrl);
                }
            } catch(Exception ex) {
                await Shell.Current.DisplayAlert("Error", $"Error al intentar abrir Waze: {ex.Message}", "Ok");
            }
        }
    }
}