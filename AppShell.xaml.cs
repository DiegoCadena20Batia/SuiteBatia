#region using directives

using BatiaSuite.Data;
using BatiaSuite.Models.EntidadesLocal.RutasEntregas;
using BatiaSuite.Services;
using BatiaSuite.Utils;
using BatiaSuite.Utils.NotificacionesSupervisor;
using BatiaSuite.ViewModel.SupervisionMantenimiento.Operarios;
using BatiaSuite.Views;
using BatiaSuite.Views.CheckListAparadores;
using BatiaSuite.Views.CheckListAparadoristasAldoConti;
using BatiaSuite.Views.ChecklistLimpieza;
using BatiaSuite.Views.ChecklistMantenimiento;
using BatiaSuite.Views.CheckListSupervisionesAldoConti;
using BatiaSuite.Views.DiarioGerenteAldoConti;
using BatiaSuite.Views.DiarioLimpieza;
using BatiaSuite.Views.Encuestas;
using BatiaSuite.Views.EntregasInteligentes;
using BatiaSuite.Views.IncidenciasBiometa;
using BatiaSuite.Views.NotificacionesSupervisores;
using BatiaSuite.Views.OrdenesTrabajo;
using BatiaSuite.Views.ReporteMantenimientoAldoConti;
using BatiaSuite.Views.RutasEntregas;
using BatiaSuite.Views.Sanitizacion;
using BatiaSuite.Views.SolicitudCotizacion;
using BatiaSuite.Views.Supervision;
using BatiaSuite.Views.SupervisionMantenimiento;
using BatiaSuite.Views.SupervisionMantenimiento.Operarios;
using BatiaSuite.Views.SupervisionMantenimiento.Supervisores;
using BatiaSuite.Views.SupplierDeliveries;
using BatiaSuite.Views.Vacantes;
using CommunityToolkit.Mvvm.Messaging;
using Plugin.LocalNotification;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#endregion

namespace BatiaSuite;

public partial class AppShell : Shell, INotifyPropertyChanged {
    private readonly IAutoSyncService _autoSyncService;
    private readonly BatiaSuite.Utils.NotificacionesSupervisor.SignalRService _signalRService;
    private bool _isSyncing = false;

    private int _conteoNotificaciones;

    public int ConteoNotificaciones {
        get => _conteoNotificaciones;
        set {
            _conteoNotificaciones = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(MostrarBadge));
        }
    }

    public bool MostrarBadge => ConteoNotificaciones > 0;
    public bool EsSupervisor { get; set; }

    // 1. Constructor por defecto (para llamadas con `new AppShell()`)
    public AppShell() : this(IPlatformApplication.Current?.Services.GetRequiredService<IAutoSyncService>()!) {
    }

    public AppShell(IAutoSyncService autoSyncService) {
        InitializeComponent();
        _autoSyncService = autoSyncService;

        EsSupervisor = UserSession.IdPuesto == 118;
        BindingContext = this;

        #region Rutas

        #region SUPPLIERDELIVERIES

        Routing.RegisterRoute(nameof(SupplierDeliveries), typeof(SupplierDeliveries));
        Routing.RegisterRoute(nameof(SupplierDeliveriesDetail), typeof(SupplierDeliveriesDetail));
        Routing.RegisterRoute(nameof(SupplierListadoMateriales), typeof(SupplierListadoMateriales));
        Routing.RegisterRoute(nameof(SupplierRegisterDelivery), typeof(SupplierRegisterDelivery));

        #endregion SUPPLIERDELIVERIES

        #region DELIVERIES

        Routing.RegisterRoute(nameof(Deliveries), typeof(Deliveries));
        Routing.RegisterRoute(nameof(DeliveriesRoute), typeof(DeliveriesRoute));
        Routing.RegisterRoute(nameof(EntregasInteligentesPage), typeof(EntregasInteligentesPage));
        Routing.RegisterRoute(nameof(DeliveriesDetail), typeof(DeliveriesDetail));
        Routing.RegisterRoute(nameof(ListadoMateriales), typeof(ListadoMateriales));
        Routing.RegisterRoute(nameof(RegisterDelivery), typeof(RegisterDelivery));

        #endregion DELIVERIES

        Routing.RegisterRoute(nameof(CorrectivosMayores), typeof(CorrectivosMayores));
        Routing.RegisterRoute(nameof(ListaCorrectivosM), typeof(ListaCorrectivosM));
        Routing.RegisterRoute(nameof(RegistrosCorrctivosM), typeof(RegistrosCorrctivosM));

        #region MANTENIMIENTO

        Routing.RegisterRoute(nameof(OrdenTrabajo), typeof(OrdenTrabajo));
        Routing.RegisterRoute(nameof(ManoObra), typeof(ManoObra));
        Routing.RegisterRoute(nameof(MaterialesUtilizados), typeof(MaterialesUtilizados));
        Routing.RegisterRoute(nameof(FotoEvidenciaPage), typeof(FotoEvidenciaPage));
        Routing.RegisterRoute(nameof(EncuestaPage), typeof(EncuestaPage));
        Routing.RegisterRoute(nameof(GenerarOrdenTrabajo), typeof(GenerarOrdenTrabajo));

        #endregion MANTENIMIENTO

        #region SUPERVISION

        Routing.RegisterRoute(nameof(SupervisionPage), typeof(SupervisionPage));
        Routing.RegisterRoute(nameof(SupervisionInmueblePage), typeof(SupervisionInmueblePage));
        Routing.RegisterRoute(nameof(MaterialesPage), typeof(MaterialesPage));
        Routing.RegisterRoute(nameof(EncuestaSupervisionPage), typeof(EncuestaSupervisionPage));
        Routing.RegisterRoute(nameof(PreguntasPage), typeof(PreguntasPage));
        Routing.RegisterRoute(nameof(EvaluacionPage), typeof(EvaluacionPage));
        Routing.RegisterRoute(nameof(ChecklistOperadorPage), typeof(ChecklistOperadorPage));
        Routing.RegisterRoute(nameof(VideoPage), typeof(VideoPage));

        #endregion SUPERVISION

        #region VACANTES

        Routing.RegisterRoute(nameof(VacantesPage), typeof(VacantesPage));
        Routing.RegisterRoute(nameof(DatosGeneralesPage), typeof(DatosGeneralesPage));
        Routing.RegisterRoute(nameof(DatosSueldoPage), typeof(DatosSueldoPage));
        Routing.RegisterRoute(nameof(DireccionPage), typeof(DireccionPage));
        Routing.RegisterRoute(nameof(DireccionFiscalPage), typeof(DireccionFiscalPage));
        Routing.RegisterRoute(nameof(DatosComplementariosPage), typeof(DatosComplementariosPage));
        Routing.RegisterRoute(nameof(DocumentosPage), typeof(DocumentosPage));

        #endregion VACANTES

        #region SANITIZACION

        Routing.RegisterRoute(nameof(SanitizacionPage), typeof(SanitizacionPage));
        Routing.RegisterRoute(nameof(EvidenciasPage), typeof(EvidenciasPage));

        #endregion SANITIZACION

        #region SUPERVISION MANTENIMIENTO

        Routing.RegisterRoute(nameof(SupervisionMantenimientoPage), typeof(SupervisionMantenimientoPage));
        Routing.RegisterRoute(nameof(SupervisionMantenimientoInmueblePage), typeof(SupervisionMantenimientoInmueblePage));
        Routing.RegisterRoute(nameof(SupervisionMantenimientoPreguntasPage), typeof(SupervisionMantenimientoPreguntasPage));
        Routing.RegisterRoute(nameof(SupervisionMantenimientoEvaluacionPage), typeof(SupervisionMantenimientoEvaluacionPage));
        Routing.RegisterRoute(nameof(SupervisionMantenimientoSeccionesPage), typeof(SupervisionMantenimientoSeccionesPage));
        Routing.RegisterRoute(nameof(SupervisionMantenimientoSeccionPage), typeof(SupervisionMantenimientoSeccionPage));
        Routing.RegisterRoute(nameof(SupervisionMantenimientoHidrantesObjectPage), typeof(SupervisionMantenimientoHidrantesObjectPage));
        Routing.RegisterRoute(nameof(SupervisionMantenimientoExtintoresObjectPage), typeof(SupervisionMantenimientoExtintoresObjectPage));
        Routing.RegisterRoute(nameof(SupervisionMantenimientoFirmasPage), typeof(SupervisionMantenimientoFirmasPage));

        #endregion

        #region Supervision Mantenimiento Tecnico

        Routing.RegisterRoute(nameof(SupervisionesMantenimientoProgramadasPage), typeof(SupervisionesMantenimientoProgramadasPage));
        Routing.RegisterRoute(nameof(SeleccionPisosPage), typeof(SeleccionPisosPage));
        Routing.RegisterRoute(nameof(SeccionesFormularioPage), typeof(SeccionesFormularioPage));
        Routing.RegisterRoute(nameof(PreguntasSeccionPage), typeof(PreguntasSeccionPage));
        Routing.RegisterRoute(nameof(SeccionesFormularioPage), typeof(SeccionesFormularioPage));
        Routing.RegisterRoute(nameof(IteracionesSeccionPage), typeof(IteracionesSeccionPage)); // <-- Nueva pantalla
        Routing.RegisterRoute(nameof(PreguntasSeccionPage), typeof(PreguntasSeccionPage));
        Routing.RegisterRoute(nameof(ResumenSupervisionPage), typeof(ResumenSupervisionPage));

        #endregion

        #region Supervision Mantenimiento Supervisor

        Routing.RegisterRoute(nameof(SupervisionMantenimientoSupervisorPage), typeof(SupervisionMantenimientoSupervisorPage));
        Routing.RegisterRoute(nameof(IteracionesSeccionSupervisorPage), typeof(IteracionesSeccionSupervisorPage));
        Routing.RegisterRoute(nameof(PreguntasSeccionSupervisorPage), typeof(PreguntasSeccionSupervisorPage));
        Routing.RegisterRoute(nameof(SeccionesFormularioSupervisorPage), typeof(SeccionesFormularioSupervisorPage));
        Routing.RegisterRoute(nameof(SeleccionPisoSupervisorPage), typeof(SeleccionPisoSupervisorPage));
        Routing.RegisterRoute(nameof(ResumenSupervisionSupervisorPage), typeof(ResumenSupervisionSupervisorPage));

        #endregion

        #region SOLICITUD COTIZACION

        Routing.RegisterRoute(nameof(SolicitudCotizacionPage), typeof(SolicitudCotizacionPage));

        #endregion SOLICITUD COTIZACION

        #region CONTROL DE APARADORES

        Routing.RegisterRoute(nameof(CheckListAparadoresInmueblePage), typeof(CheckListAparadoresInmueblePage));
        Routing.RegisterRoute(nameof(CheckListAparadoresPreguntasUnoPage), typeof(CheckListAparadoresPreguntasUnoPage));
        Routing.RegisterRoute(nameof(CheckListAparadoresPreguntasDosPage), typeof(CheckListAparadoresPreguntasDosPage));
        Routing.RegisterRoute(nameof(CheckListAparadoresPreguntasTresPage), typeof(CheckListAparadoresPreguntasTresPage));
        Routing.RegisterRoute(nameof(CheckListAparadoresPreguntasCuatroPage), typeof(CheckListAparadoresPreguntasCuatroPage));
        Routing.RegisterRoute(nameof(CheckListAparadoresPreguntasCincoPage), typeof(CheckListAparadoresPreguntasCincoPage));
        Routing.RegisterRoute(nameof(CheckListAparadoresPreguntasResumenPage), typeof(CheckListAparadoresPreguntasResumenPage));

        #endregion CONTROL DE APARADORES

        #region INCIDENCIAS BIOMETA

        Routing.RegisterRoute(nameof(IncidenciasBiometaPage), typeof(IncidenciasBiometaPage));

        #endregion INCIDENCIAS BIOMETA

        #region SUPERVICION_ALDOCONTI

        Routing.RegisterRoute(nameof(ChecklistPage), typeof(ChecklistPage));
        Routing.RegisterRoute(nameof(AparadoristasPage), typeof(AparadoristasPage));
        Routing.RegisterRoute(nameof(LimpiezaPage), typeof(LimpiezaPage));
        Routing.RegisterRoute(nameof(MantenimientoPage), typeof(MantenimientoPage));
        Routing.RegisterRoute(nameof(DiarioGerentePage), typeof(DiarioGerentePage));
        Routing.RegisterRoute(nameof(ReporteMantenimientoPage), typeof(ReporteMantenimientoPage));
        Routing.RegisterRoute(nameof(DiarioLimpiezaPage), typeof(DiarioLimpiezaPage));

        #endregion SUPERVICION_ALDOCONTI

        Routing.RegisterRoute(nameof(TiposListadoPage), typeof(TiposListadoPage));
        Routing.RegisterRoute(nameof(CentroNotificacionesSupervisor), typeof(CentroNotificacionesSupervisor));

        #endregion Rutas

        Connectivity.Current.ConnectivityChanged += OnConnectivityChanged;

        WeakReferenceMessenger.Default.Register<NotificationCountMessage>(this, (r, m) => {
            MainThread.BeginInvokeOnMainThread(() => {
                ConteoNotificaciones = m.Value;
            });
        });

        if(EsSupervisor) {
            string idSupervisorLogueado = UserSession.IdPersonal.ToString();

            _signalRService = new Utils.NotificacionesSupervisor.SignalRService(idSupervisorLogueado);
            Task.Run(async () => await _signalRService.ConectarAsync());
        }
    }

    protected override async void OnAppearing() {
        base.OnAppearing();
        await SolicitarPermisoNotificacionesAsync();
    }

    private async void OnNotificationBellTapped(object sender, EventArgs e) {
        await Shell.Current.GoToAsync("CentroNotificacionesSupervisor");
    }

    private async void OnConnectivityChanged(object? sender, ConnectivityChangedEventArgs e) {
        if(e.NetworkAccess == NetworkAccess.Internet && !_isSyncing) {
            await Task.Run(async () => {
                try {
                    _isSyncing = true;
                    System.Diagnostics.Debug.WriteLine("[Automated_Sync] Conexión a Internet detectada. Ejecutando motor de sincronización...");

                    // Toda la lógica de iteración, notificaciones y envío se realiza aquí
                    await _autoSyncService.SincronizarTodoAsync();
                } catch(Exception ex) {
                    System.Diagnostics.Debug.WriteLine($"[Automated_Sync_Error] Error de sincronización: {ex.Message}");
                } finally {
                    _isSyncing = false;
                }
            });
        }
    }

    public new event PropertyChangedEventHandler? PropertyChanged;

    protected new void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void Dispose() {
        Connectivity.Current.ConnectivityChanged -= OnConnectivityChanged;
        GC.SuppressFinalize(this);
    }

    private async Task SolicitarPermisoNotificacionesAsync() {
        var status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();

        if(status != PermissionStatus.Granted) {
            status = await Permissions.RequestAsync<Permissions.PostNotifications>();
        }

        if(status == PermissionStatus.Granted) {
            System.Diagnostics.Debug.WriteLine("[Permisos] Notificaciones permitidas.");
        }
    }
}