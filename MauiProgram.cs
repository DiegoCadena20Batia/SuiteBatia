using BatiaSuite.Data;
using BatiaSuite.Resources.IconFonts;
using BatiaSuite.Views;
using Camera.MAUI;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;
using Mopups.Hosting;
using Shiny;
using BatiaSuite.Services;
using BatiaSuite.ViewModel.EntregasInteligentes;
using BatiaSuite.Views.EntregasInteligentes;
using Shiny.Infrastructure;
using Shiny.Locations;
using BatiaSuite.Utils;
using BatiaSuite.ViewModel;
using BatiaSuite.ViewModel.SupplierDeliveries;
using BatiaSuite.Views.SupplierDeliveries;
//using Plugin.Maui.SegmentedControl;
using BatiaSuite.ViewModel.CheckListAparadores;
using BatiaSuite.Views.CheckListAparadores;
using BatiaSuite.ViewModel.Supervisionmantenimiento;
using BatiaSuite.Views.SupervisionMantenimiento;
using BatiaSuite.Models.SupervisionMantenimiento;
using BatiaSuite.ViewModel.SupervisionMantenimiento;
namespace BatiaSuite;
public static class MauiProgram {
    public static MauiApp CreateMauiApp() {

        var builder = MauiApp.CreateBuilder();
        
        builder
            .UseMauiApp<App>()
            //.UseSegmentedControl()
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
//#if DEBUG
//#if !IOS
//        builder.Logging.AddDebug();
//#endif
//#endif
//#if IOS
//        builder.Logging.AddConsole();
//#endif

        // Para configurar servicios de navegación si los usas
        //builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingleton<IMediaPicker>(MediaPicker.Default);
        builder.Services.AddTransient<RegisterDelivery>();
        builder.Services.AddTransient<SupplierRegisterDelivery>();
        builder.Services.AddTransient<ListaCorrectivosM>();
        builder.Services.AddSingleton<DbContext>();
        builder.Services.AddTransient<EntregasInteligentesViewModel>();
        builder.Services.AddTransient<EntregasInteligentesPage>();
        builder.Services.AddTransient<DeliveriesViewModel>();
        builder.Services.AddTransient<Deliveries>();
        builder.Services.AddTransient<DeliveriesRoute>();

        // CHECKLIST APARADORES
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

        //SUPERVISION MANTENIMIENTO

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

#if ANDROID || IOS
        builder.Services.AddGps<MyGpsDelegate>();
#endif

        return builder.Build();

    }
}