using BatiaSuite.Data;
using BatiaSuite.Models.EntidadesLocal.RutasEntregas;
using BatiaSuite.Views;
using BatiaSuite.Views.CheckListAparadores;
using BatiaSuite.Views.CheckListAparadoristasAldoConti;
using BatiaSuite.Views.ChecklistLimpieza;
using BatiaSuite.Views.ChecklistMantenimiento;
using BatiaSuite.Views.ChecklistMonitoreo;
using BatiaSuite.Views.CheckListSupervisionesAldoConti;
using BatiaSuite.Views.DiarioGerenteAldoConti;
using BatiaSuite.Views.DiarioLimpieza;
using BatiaSuite.Views.Encuestas;
using BatiaSuite.Views.EntregasInteligentes;
using BatiaSuite.Views.IncidenciasBiometa;
using BatiaSuite.Views.OrdenesTrabajo;
using BatiaSuite.Views.ReporteMantenimientoAldoConti;
using BatiaSuite.Views.RutasEntregas;
using BatiaSuite.Views.Sanitizacion;
using BatiaSuite.Views.SolicitudCotizacion;
using BatiaSuite.Views.Supervision;
using BatiaSuite.Views.SupervisionMantenimiento;
using BatiaSuite.Views.SupplierDeliveries;
using BatiaSuite.Views.Vacantes;
using Microsoft.Maui.Networking;

namespace BatiaSuite;

public partial class AppShell : Shell {

    private readonly SyncService _syncService;
    private bool _isSyncing = false;

    public AppShell() {
        InitializeComponent();

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

        #endregion SUPERVISION MANTENIMIENTO

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
        Routing.RegisterRoute(nameof(MantenimientoPage), typeof(MantenimientoPage));
        Routing.RegisterRoute(nameof(DiarioGerentePage), typeof(DiarioGerentePage));
        Routing.RegisterRoute(nameof(ReporteMantenimientoPage), typeof(ReporteMantenimientoPage));
        Routing.RegisterRoute(nameof(DiarioLimpiezaPage), typeof(DiarioLimpiezaPage));

        #endregion SUPERVICION_ALDOCONTI

        Routing.RegisterRoute(nameof(TiposListadoPage), typeof(TiposListadoPage));

       
        _syncService = new SyncService();
        Connectivity.Current.ConnectivityChanged += OnConnectivityChanged;
    }

    private async void OnConnectivityChanged(object? sender, ConnectivityChangedEventArgs e) {
        if(e.NetworkAccess == NetworkAccess.Internet && !_isSyncing) {
            try {
                _isSyncing = true;
                System.Diagnostics.Debug.WriteLine("[Automated_Sync] Conexión detectada. Procesando cola de entregas...");

                // Despacha de forma asíncrona la cola específica de entregas de materiales
                await _syncService.ProcesarPendientesAsync<RutaInmueblePendiente>();

                System.Diagnostics.Debug.WriteLine("[Automated_Sync] Sincronización automática de entregas completada.");
            } catch(Exception ex) {
                System.Diagnostics.Debug.WriteLine($"[Automated_Sync_Error] Error de sincronización: {ex.Message}");
            } finally {
                _isSyncing = false;
            }
        }
    }

    ~AppShell() {
        Connectivity.Current.ConnectivityChanged -= OnConnectivityChanged;
    }
}