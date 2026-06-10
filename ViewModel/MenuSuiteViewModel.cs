using BatiaSuite.Models;
using BatiaSuite.Models.Apps;
using BatiaSuite.Utils;
using BatiaSuite.Views;
using BatiaSuite.Views.CheckListAparadores;
using BatiaSuite.Views.CheckListAparadoristasAldoConti;
using BatiaSuite.Views.ChecklistLimpieza;
using BatiaSuite.Views.ChecklistMantenimiento;
using BatiaSuite.Views.ChecklistMonitoreo;
using BatiaSuite.Views.CheckListSupervisionesAldoConti;
using BatiaSuite.Views.EntregasInteligentes;
using BatiaSuite.Views.IncidenciasBiometa;
using BatiaSuite.Views.OrdenesTrabajo;
using BatiaSuite.Views.Sanitizacion;
using BatiaSuite.Views.SolicitudCotizacion;
using BatiaSuite.Views.Supervision;
using BatiaSuite.Views.SupervisionMantenimiento;
using BatiaSuite.Views.SupplierDeliveries;
using BatiaSuite.Views.Vacantes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

//using static SQLite.SQLite3;

namespace BatiaSuite.ViewModel;

public partial class MenuSuiteViewModel : ViewModelBase {
    private readonly HttpClient _httpClient;
    private readonly string _apiKey = "AIzaSyBpoRJAkG1NFtHFuE3uTRbVnRcTG7ndz18";

    [ObservableProperty]
    private ObservableCollection<Monkey> _monkeys;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(NextPageCommand))]
    private bool _isBusy;

    [ObservableProperty]
    private string _appVersionString;

    [ObservableProperty]
    private string _appName;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isRefreshing;

    private OrdenesSupervisionTotal _ordenesSupervisionTotal;

    public MenuSuiteViewModel() {
        AppName = AppInfo.Name;
        AppVersionString = AppInfo.VersionString;
        InitValuesAsync();
    }

    public async Task<bool> ValidarVersion() {
        try {
            string version = AppInfo.Current.VersionString;
            string buildVersion = AppInfo.Current.BuildString;
            string plataforma = "";

#if ANDROID
            plataforma = "1";
#endif
#if IOS
plataforma = "2";
#endif

            string url = Constants.API_BASE_URL + $"VersionesApp?app=2&plataforma={plataforma}";

            var _httpClient = new HttpClient();
            var response = await _httpClient.GetAsync(url);
            if(!response.IsSuccessStatusCode) {
                Console.WriteLine("No se pudo obtener la versión de la app desde el server");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = System.Text.Json.JsonSerializer.Deserialize<List<VersionApp>>(jsonResponse);

            if(result != null && result[0] != null) {
                if(result[0].nomversion == version) {
                    return true;
                } else {
                    Console.WriteLine($"version incorrecta: {result[0].nomversion} (num: {result[0].numversion}), se esperaba {version}");
                }
            }

            return true;
        } catch(Exception ex) {
            Console.WriteLine($"Error al validar la versión de la app: {ex.Message}");
            return true;
        }
    }

    [RelayCommand]
    private async Task RefreshPage() {
        IsRefreshing = true;
        if(InternetUtil.IsConnectedInternet()) {
            try {
                ModulosResponse modulos = await _httpHelper.GetAsync<ModulosResponse>(Constants.GET_MODULOS_MENU + "?idpersonal=" + UserSession.IdPersonal.ToString());
                string modulosSession;
                if(modulos != null) {
                    if(modulos.Modulo != null && modulos.Modulo.Count > 0) {
                        modulosSession = string.Join(",", modulos.Modulo);
                        UserSession.Modulos = modulosSession;
                        Monkeys = await GenerarMenu();
                    }
                } else {
                    await App.Current.MainPage.DisplayAlert("Alerta", "Sin modulos asignados, verifique con su encargado", Constants.ACEPTAR);
                }
            } catch(Exception ex) {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, Constants.ACEPTAR);
            }
        } else {
            await App.Current.MainPage.DisplayAlert("Error", "Verifique su cónexion a internet.", Constants.ACEPTAR);
        }
        IsRefreshing = false;
    }

    private async void InitValuesAsync() {
        IsBusy = true;
        IsLoading = true;
        DateTime currentDate = DateTime.Now;

        if(!InternetUtil.IsConnectedInternet()) {
            _ordenesSupervisionTotal = new OrdenesSupervisionTotal();
            if(UserSession.Modulos == "" && UserSession.Modulos == null) {
            }
        } else {
            string url = $"{Constants.SUP_GET_ORDENES_TOTALES_API}?idsupervisor={UserSession.IdEmpleado}&anio={currentDate.Year}&mes={currentDate.Month}";

            _ordenesSupervisionTotal = await _httpHelper.GetAsync<OrdenesSupervisionTotal>(url);

            if(_ordenesSupervisionTotal is null || _ordenesSupervisionTotal.SupTotales == 0) {
                _ordenesSupervisionTotal = new OrdenesSupervisionTotal();
            } else {
                _ordenesSupervisionTotal.Width = 35;
            }

            ModulosResponse modulos = await _httpHelper.GetAsync<ModulosResponse>(Constants.GET_MODULOS_MENU + "?idpersonal=" + UserSession.IdPersonal.ToString());
            string modulosSession;
            if(modulos != null) {
                if(modulos.Modulo != null && modulos.Modulo.Count > 0) {
                    modulosSession = string.Join(",", modulos.Modulo);
                    UserSession.Modulos = modulosSession;
                }
            } else {
                modulosSession = "";
                UserSession.Modulos = modulosSession;
            }
        }
        Monkeys = await GenerarMenu();
        await ValidarVersion();
        IsBusy = false;
        IsLoading = false;
        //CreateMonkeyCollection();
    }

    public class ModulosResponse {

        [JsonProperty("modulo")]
        public List<int> Modulo { get; set; }
    }

    private void CreateMonkeyCollection() {
        Monkeys = new ObservableCollection<Monkey> {
            new Monkey {
                Name =Constants.ENTREGAS,
                Details = Constants.DETALLES_ENTREGAS,
                ImageUrl = "iconoentregaspng.png"
            },
            new Monkey {
                Name = Constants.CORRECTIVOS_MAYORES,
                Details = Constants.DETALLES_CORRECTIVOS_MAYORES,
                ImageUrl = "logo_correctivos.png"
            },
            new Monkey {
                Name = Constants.MANTENIMIENTO,
                Details = Constants.DETALLES_CORRECTIVOS_MENORES,
                ImageUrl = "mantenimiento.png"
            },
            new Monkey {
                Name = Constants.SUPERVISION,
                Details = Constants.DETALLES_SUPERVISION,
                ImageUrl = "supervision.png",
                OrdenesTotales = _ordenesSupervisionTotal
            },
            new Monkey {
                Name = Constants.VACANTES,
                Details = Constants.DETALLES_VACANTES,
                ImageUrl = "vacantes.png"
            },
            new Monkey {
                Name = Constants.SANITIZACION,
                Details = Constants.DETALLES_SANITIZACION,
                ImageUrl = "sanitizacion.png"
            }
       };
    }

    public async Task<ObservableCollection<Monkey>> GenerarMenu() {
        // ! VERIFICAR POSIBLES NULL Y TAMBIEN RETORNOS PARA USUARIOS DESACTUALIZADOS

        List<int> listaModulos = new();

        if(!string.IsNullOrEmpty(UserSession.Modulos)) {
            listaModulos = UserSession.Modulos
                .Split(',')
                .Where(x => int.TryParse(x, out _))
                .Select(int.Parse)
                .ToList();
        }

        // Si no tiene módulos asignados, devuelve una lista vacía o lanza una advertencia
        if(listaModulos.Count == 0) {
            // Podrías mostrar un mensaje o una vista especial indicando que no hay permisos
            return new ObservableCollection<Monkey>();
        }
        //PARA USUARIOS QUE AUN NO TIENEN MODULOS ASIGNADOS EN EL BACKEND NO PODRAN VER EL MENU DE SUITE BATIA
        //APARECIENDO UNA LEYENDA INDICANDO QUE SE DEBEN CONTACTAR CON SISTEMAS PARA DAR DE ALTA SUS PERMISOS

        // Lista completa de posibles módulos
        var todosLosMonos = new List<Monkey>
        {
        new Monkey {
            Name = Constants.ENTREGAS,
            Details = Constants.DETALLES_ENTREGAS,
            ImageUrl = "iconoentregaspng.png",
            IdModulo = 1
        },
        new Monkey {
            Name = Constants.CORRECTIVOS_MAYORES,
            Details = Constants.DETALLES_CORRECTIVOS_MAYORES,
            ImageUrl = "logo_correctivos.png",
            IdModulo = 2
        },
        new Monkey {
            Name = Constants.MANTENIMIENTO,
            Details = Constants.DETALLES_CORRECTIVOS_MENORES,
            ImageUrl = "mantenimiento.png",
            IdModulo = 3
        },
        new Monkey {
            Name = Constants.SUPERVISION,
            Details = Constants.DETALLES_SUPERVISION,
            ImageUrl = "supervision.png",
            OrdenesTotales = _ordenesSupervisionTotal,
            IdModulo = 4
        },
        new Monkey {
            Name = Constants.VACANTES,
            Details = Constants.DETALLES_VACANTES,
            ImageUrl = "vacantes.png",
            IdModulo = 5
        },
        new Monkey {
            Name = Constants.SANITIZACION,
            Details = Constants.DETALLES_SANITIZACION,
            ImageUrl = "sanitizacion.png",
            IdModulo = 6
        },
        new Monkey {
            Name = "CHECKLIST CONTROL DE APARADORES",
            Details = "Evaluación y supervisión para estándares de presentación comercial",
            ImageUrl = "supervisionmantenimiento.png",
            IdModulo = 9
        },
        new Monkey {
            Name = "INCIDENCIAS DIARIAS",
            Details = "Visualizar incidencias de asistencia en los puntos de atencion asignados.",
            ImageUrl = "incidenciasbiometa.png",
            IdModulo = 10
        }
        ,
        new Monkey {
            Name = Constants.SUPERVISION_MANTENIMIENTO,
            Details = Constants.DETALLES_SUPERVISION_MANTENIMIENTO,
            ImageUrl = "supervisionmantenimientoiconnew.png",
            IdModulo = 7
        },
        new Monkey {
            Name = Constants.SUPERVISION_ALDO_CONTI,
            Details = Constants.DETALLES_SUPERVISION_ALDO_CONTI,
            ImageUrl = "supervisionaldoconti.png",
            IdModulo = 11
        },
        new Monkey {
            Name = Constants.APARADORISTAS_ALDO_CONTI,
            Details = Constants.DETALLES_APARADORISTAS_ALDO_CONTI,
            ImageUrl = "aparadoristas.png",
            IdModulo = 12
        },
        new Monkey {
            Name = Constants.LIMPIEZA_ALDO_CONTI,
            Details = Constants.DETALLES_LIMPIEZA_ALDO_CONTI,
            ImageUrl = "limpieza.png",
            IdModulo = 13
        },
        new Monkey {
            Name = Constants.MANTENIMIENTO_ALDO_CONTI,
            Details = Constants.DETALLES_MANTENIMIENTO_ALDO_CONTI,
            ImageUrl = "mantenimientoac.png",
            IdModulo = 14
        },
        new Monkey {
            Name = Constants.MONITOREO_ALDO_CONTI,
            Details = Constants.DETALLES_MONITOREO_ALDO_CONTI,
            ImageUrl = "monitoreo.png",
            IdModulo = 15
        }
        //,
        //new Monkey {
        //    Name = Constants.SOLICITUD_COTIZACION,
        //    Details = Constants.SOLICITUD_COTIZACION_DETALLE,
        //    ImageUrl = "solicitudcotizacion.png",
        //    IdModulo = 8
        //}
    };

        // Filtra los monos según los módulos permitidos
        var monosFiltrados = todosLosMonos
            .Where(m => listaModulos.Contains(m.IdModulo))
            .ToList();

        return new ObservableCollection<Monkey>(monosFiltrados);
    }

    [RelayCommand(CanExecute = nameof(CanExecuteNextPage))]
    public async Task NextPage(string route) {
        //IsBusy = true;
        //IsLoading = true;

        switch(route) {
            case Constants.ENTREGAS:
                if(UserSession.IdCliente != 0) {
                    route = $"{nameof(SupplierDeliveries)}";  //MODELO ANTERIOR DE ENTREGAS
                } else {
                    route = $"{nameof(Deliveries)}";  //NUEVO MODELO DE ENTREGAS
                }

                //route = $"{nameof(EntregasInteligentesPage)}";    //RUTA PARA NUEVO MODULO DE ENTREGAS
                break;

            case Constants.CORRECTIVOS_MAYORES:
                route = $"{nameof(CorrectivosMayores)}";
                break;

            case Constants.MANTENIMIENTO:
                if(UserSession.IdCliente == 1) {
                    route = $"{nameof(GenerarOrdenTrabajo)}";
                } else {
                    route = $"{nameof(OrdenTrabajo)}";
                }

                break;

            case Constants.SUPERVISION:
                route = $"{nameof(SupervisionPage)}";
                break;

            case Constants.VACANTES:
                route = $"{nameof(VacantesPage)}";
                break;

            case Constants.SANITIZACION:
                route = $"{nameof(SanitizacionPage)}";
                break;

            case Constants.SUPERVISION_MANTENIMIENTO:
                route = $"{nameof(SupervisionMantenimientoPage)}";
                break;

            case Constants.SOLICITUD_COTIZACION:
                route = $"{nameof(SolicitudCotizacionPage)}";
                break;

            case "CHECKLIST CONTROL DE APARADORES":
                route = $"{nameof(CheckListAparadoresInmueblePage)}";
                break;

            case "INCIDENCIAS DIARIAS":
                route = $"{nameof(IncidenciasBiometaPage)}";
                break;

            case Constants.SUPERVISION_ALDO_CONTI:
                route = $"{nameof(ChecklistPage)}";
                break;

            case Constants.APARADORISTAS_ALDO_CONTI:
                route = $"{nameof(AparadoristasPage)}";
                break;

            case Constants.LIMPIEZA_ALDO_CONTI:
                route = $"{nameof(LimpiezaPage)}";
                break;

            case Constants.MANTENIMIENTO_ALDO_CONTI:
                route = $"{nameof(MantenimientoPage)}";
                break;

            case Constants.MONITOREO_ALDO_CONTI:
                route = $"{nameof(MonitoreoPage)}";
                break;

            default:
                IsBusy = false;
                IsLoading = false;
                return;
        }
        await Shell.Current.GoToAsync(route, true);
        //await Constants.GoToAsync(route);

        IsBusy = false;
        IsLoading = false;
    }

    [RelayCommand]
    private async Task Logout() {
        IsLoading = true;
        await Task.Delay(200);
        UserSession.ClearSession();
        App.Current.MainPage = new Logueo();
        IsLoading = false;
    }

    private bool CanExecuteNextPage() {
        return !IsBusy;
    }

    public async Task<string> ObtenerRutaAsync(string origen, string destino, List<string> waypoints = null) {
        origen = "19.617324, -99.289483";
        destino = "19.36478456915744, -99.1724843795433";

        var url = $"https://maps.googleapis.com/maps/api/directions/json?origin={origen}&destination={destino}&key={_apiKey}";

        if(waypoints != null && waypoints.Count > 0) {
            var wp = string.Join("|", waypoints);
            url += $"&waypoints=optimize:true|{wp}";
        }

        var response = await _httpClient.GetAsync(url);

        if(!response.IsSuccessStatusCode)
            throw new Exception("Error al consultar la API de Google Maps");

        return await response.Content.ReadAsStringAsync();
    }
}