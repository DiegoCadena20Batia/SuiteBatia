using BatiaSuite.Models.OrdenesTrabajo;
using BatiaSuite.Models.SupervisionMantenimiento.Operarios;
using BatiaSuite.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.ViewModel.SupervisionMantenimiento.Operarios
{
    public partial class SupervisionMantenimientoOperarioViewModel : ObservableObject, IQueryAttributable  {
        private readonly HttpHelper _httpHelper;

        private string baseUrlApi = Constants.API_BASE_URL;

        [ObservableProperty]
        private OrdenTrabajoModel _orden;

        [ObservableProperty]
        private bool _isLoadingSecciones;

        public ObservableCollection<SeccionModel> Secciones { get; } = new();

        public void ApplyQueryAttributes(IDictionary<string, object> query) {
            if(query.TryGetValue("OrdenSeleccionada", out var ordenObj) && ordenObj is OrdenTrabajoModel ordenValida) {
                Orden = ordenValida;
            }
        }

        public SupervisionMantenimientoOperarioViewModel(HttpHelper httpHelper) {
            _httpHelper = httpHelper;
        }

        partial void OnOrdenChanged(OrdenTrabajoModel value) {
            if(value != null) {
                _ = CargarSeccionesAsync();
            }
        }

        [RelayCommand]
        private async Task CargarSeccionesAsync() {
            if(IsLoadingSecciones) return;

            try {
                IsLoadingSecciones = true;
                Secciones.Clear();

                // Petición al endpoint
                string url = $"{baseUrlApi}SupervisionMantenimeintoChecklist";
                var resultado = await _httpHelper.GetAsync<List<SeccionModel>>(url);

                if(resultado != null) {
                    foreach(var seccion in resultado) {
                        Secciones.Add(seccion);
                    }
                }
            } catch(Exception ex) {
                System.Diagnostics.Debug.WriteLine($"Error al consumir el checklist: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "No se pudo cargar el checklist de supervisión.", "OK");
            } finally {
                IsLoadingSecciones = false;
            }
        }

        [RelayCommand]
        private async Task AbrirPreguntasSeccionAsync(SeccionModel seccionSeleccionada) {
            IsLoadingSecciones = true;
            if(seccionSeleccionada == null) return;

            var navParameters = new Dictionary<string, object>
            {
            { "SeccionSeleccionada", seccionSeleccionada },
            { "IdOrden", Orden?.idOrden ?? 0 }
        };

            await Shell.Current.GoToAsync("PreguntasSeccionPage", navParameters);
            IsLoadingSecciones = false;
        }
    }
}
