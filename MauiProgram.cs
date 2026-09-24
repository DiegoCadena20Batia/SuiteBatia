using BatiaSuite.Data;
using BatiaSuite.Interfaz;
using BatiaSuite.Interfaz.Repositories;
using BatiaSuite.Repositories;
using BatiaSuite.Resources.IconFonts;
using BatiaSuite.Services;
using BatiaSuite.Services.SupervisionesMantenimiento;
using BatiaSuite.SyncHandlers;
using BatiaSuite.Utils;
using BatiaSuite.ViewModel;
using BatiaSuite.ViewModel.CheckListAparadores;
using BatiaSuite.ViewModel.EntregasInteligentes;
using BatiaSuite.ViewModel.NotificacionesSupervisores;
using BatiaSuite.ViewModel.RutasEntregas;
using BatiaSuite.ViewModel.Supervisionmantenimiento;
using BatiaSuite.ViewModel.SupervisionMantenimiento;
using BatiaSuite.ViewModel.SupervisionMantenimiento.Operarios;
using BatiaSuite.ViewModel.SupervisionMantenimiento.Supervisores;
using BatiaSuite.Views;
using BatiaSuite.Views.CheckListAparadores;
using BatiaSuite.Views.EntregasInteligentes;
using BatiaSuite.Views.NotificacionesSupervisores;
using BatiaSuite.Views.RutasEntregas;
using BatiaSuite.Views.SupervisionMantenimiento;
using BatiaSuite.Views.SupervisionMantenimiento.Operarios;
using BatiaSuite.Views.SupervisionMantenimiento.Supervisores;
using BatiaSuite.Views.SupplierDeliveries;
using Camera.MAUI;
using CommunityToolkit.Maui;
using Mopups.Hosting;
using Plugin.LocalNotification;
using Shiny;

namespace BatiaSuite;

public static class MauiProgram {

    public static MauiApp CreateMauiApp() {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseLocalNotification()
            .UseMauiMaps()
            .UseShiny()
            .UseMauiCommunityToolkitMediaElement()
            .UseMauiCommunityToolkit()
            .ConfigureMopups()
            .UseMauiCameraView()
            .ConfigureFonts(fonts => {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Montserrat-Regular.ttf", "MontRegular");
                fonts.AddFont("Montserrat-Bold.ttf", "MontBold");
                fonts.AddFont("Montserrat-SemiBold.ttf", "MontSemibold");
                fonts.AddFont("Montserrat-ExtraBold.ttf", "MontExtrabold");
                fonts.AddFont("icons.ttf", Icons.Family);
                fonts.AddFont("Font Awesome 7 Free_Solid-900.otf", "FASolid");
            });

        // Servicios Base
        builder.Services.AddSingleton<IMediaPicker>(MediaPicker.Default);
        builder.Services.AddSingleton<DbContext>();
        builder.Services.AddSingleton<HttpHelper>();
        builder.Services.AddSingleton<LocalDatabaseService>();
        builder.Services.AddSingleton<SupervisionStateService>();

        // Módulo: Entregas
        builder.Services.AddTransient<RegisterDelivery>();
        builder.Services.AddTransient<SupplierRegisterDelivery>();
        builder.Services.AddTransient<EntregasInteligentesViewModel>();
        builder.Services.AddTransient<EntregasInteligentesPage>();
        builder.Services.AddTransient<DeliveriesViewModel>();
        builder.Services.AddTransient<Deliveries>();
        builder.Services.AddTransient<DeliveriesRoute>();
        builder.Services.AddTransient<DeliveriesDetailViewModel>();
        builder.Services.AddTransient<DeliveriesDetail>();
        builder.Services.AddTransient<TiposListadoPage>();
        builder.Services.AddTransient<TiposListadoViewModel>();

        // Módulo: Checklist Aparadores
        builder.Services.AddSingleton<CheckListService>();
        builder.Services.AddTransient<CheckListAparadoresInmuebleViewModel>();
        builder.Services.AddTransient<CheckListAparadoresInmueblePage>();
        builder.Services.AddTransient<CheckListAparadoresPreguntasUnoViewModel>();
        builder.Services.AddTransient<CheckListAparadoresPreguntasUnoPage>();
        builder.Services.AddTransient<CheckListAparadoresPreguntasDosViewModel>();
        builder.Services.AddTransient<CheckListAparadoresPreguntasDosPage>();
        builder.Services.AddTransient<CheckListAparadoresPreguntasTresViewModel>();
        builder.Services.AddTransient<CheckListAparadoresPreguntasTresPage>();
        builder.Services.AddTransient<CheckListAparadoresPreguntasCuatroViewModel>();
        builder.Services.AddTransient<CheckListAparadoresPreguntasCuatroPage>();
        builder.Services.AddTransient<CheckListAparadoresPreguntasCincoViewModel>();
        builder.Services.AddTransient<CheckListAparadoresPreguntasCincoPage>();
        builder.Services.AddTransient<CheckListAparadoresPreguntasResumenViewModel>();
        builder.Services.AddTransient<CheckListAparadoresPreguntasResumenPage>();

        // Módulo: Supervisión Mantenimiento (General)
        builder.Services.AddSingleton<SupervisionMantenimientoService>();
        builder.Services.AddTransient<SupervisionMantenimientoInmuebleViewModel>();
        builder.Services.AddTransient<SupervisionMantenimientoInmueblePage>();
        builder.Services.AddTransient<SupervisionMantenimientoPreguntasViewModel>();
        builder.Services.AddTransient<SupervisionMantenimientoPreguntasPage>();
        builder.Services.AddTransient<SupervisionMantenimientoSeccionesViewModel>();
        builder.Services.AddTransient<SupervisionMantenimientoSeccionesPage>();
        builder.Services.AddTransient<SupervisionMantenimientoSeccionViewModel>();
        builder.Services.AddTransient<SupervisionMantenimientoSeccionPage>();
        builder.Services.AddTransient<SupervisionMantenimientoHidrantesObjectViewModel>();
        builder.Services.AddTransient<SupervisionMantenimientoHidrantesObjectPage>();
        builder.Services.AddTransient<SupervisionMantenimientoExtintoresObjectViewModel>();
        builder.Services.AddTransient<SupervisionMantenimientoExtintoresObjectPage>();
        builder.Services.AddTransient<SupervisionMantenimientoFirmasViewModel>();
        builder.Services.AddTransient<SupervisionMantenimientoFirmasPage>();
        builder.Services.AddTransient<SupervisionesMantenimientoProgramadasViewModel>();
        builder.Services.AddTransient<SupervisionesMantenimientoProgramadasPage>();
        builder.Services.AddTransient<SeleccionPisosPage>();
        builder.Services.AddTransient<SeleccionPisosViewModel>();

        // Módulo: Supervisión Mantenimiento Técnico
        builder.Services.AddTransient<SeccionesFormularioPage>();
        builder.Services.AddTransient<SeccionesFormularioViewModel>();
        builder.Services.AddTransient<PreguntasSeccionPage>();
        builder.Services.AddTransient<PreguntasSeccionViewModel>();
        builder.Services.AddTransient<IteracionesSeccionPage>();
        builder.Services.AddTransient<IteracionesSeccionViewModel>();
        builder.Services.AddTransient<ResumenSupervisionViewModel>();
        builder.Services.AddTransient<ResumenSupervisionPage>();

        // Módulo: Supervisión Mantenimiento Supervisor
        builder.Services.AddTransient<SupervisionMantenimientoSupervisorPage>();
        builder.Services.AddTransient<SupervisionMantenimientoSupervisorViewModel>();
        builder.Services.AddTransient<IteracionesSeccionSupervisorViewModel>();
        builder.Services.AddTransient<PreguntasSeccionSupervisorViewModel>();
        builder.Services.AddTransient<SeccionesFormularioSupervisorViewModel>();
        builder.Services.AddTransient<SeleccionPisoSupervisorViewModel>();
        builder.Services.AddTransient<ResumenSupervisionSupervisorViewModel>();
        #endregion




        builder.Services.AddSingleton<SupervisionStateService>();



        builder.Services.AddSingleton<DbContext>();
        builder.Services.AddSingleton<LocalDbContext>();
        builder.Services.AddSingleton<IOrdenesRepository, OrdenesRepository>();
        builder.Services.AddSingleton<IPlantillasRepository, PlantillasRepository>();
        builder.Services.AddSingleton<ITiposServicioRepository, TiposServicioRepository>();
        builder.Services.AddSingleton<IClientesRepository, ClientesRepository>();
        builder.Services.AddSingleton<IInmueblesRepository, InmueblesRepository>();

        // Módulo: Correctivos Mayores y Notificaciones
        builder.Services.AddTransient<ListaCorrectivosM>();
        builder.Services.AddTransient<CorrectivosMayoresViewModel>();
        builder.Services.AddTransient<CentroNotificacionesSupervisorViewModel>();
        builder.Services.AddTransient<CentroNotificacionesSupervisor>();

#if ANDROID || IOS
        builder.Services.AddGps<MyGpsDelegate>();
#endif

        return builder.Build();
    }
}