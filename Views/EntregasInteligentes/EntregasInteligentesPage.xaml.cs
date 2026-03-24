using BatiaSuite.ViewModel.EntregasInteligentes;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using System.Collections.Specialized;

namespace BatiaSuite.Views.EntregasInteligentes;

public partial class EntregasInteligentesPage : ContentPage {
    EntregasInteligentesViewModel viewModel;
    public EntregasInteligentesPage(EntregasInteligentesViewModel viewModel) {
        InitializeComponent();
        this.viewModel = viewModel;
        BindingContext = viewModel;
        viewModel.RutaCoordenadas.CollectionChanged += RutaCoordenadas_CollectionChanged;
    }

    private void RutaCoordenadas_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
        RutaMap.MapElements.Clear(); // Limpia rutas anteriores

        var polyline = new Polyline {
            StrokeColor = Colors.Blue,
            StrokeWidth = 15
        };

        foreach(var location in viewModel.RutaCoordenadas) {
            polyline.Geopath.Add(location);
        }

        RutaMap.MapElements.Add(polyline);

        if(viewModel.RutaCoordenadas.Count > 0) {
            // Centrar mapa en el primer punto de la ruta
            var start = viewModel.RutaCoordenadas[0];
            RutaMap.MoveToRegion(MapSpan.FromCenterAndRadius(start, Distance.FromKilometers(5)));
        }
    }

    protected override void OnAppearing() {
        base.OnAppearing();

        var location = new Location(19.36478522971544, -99.17254690402716);
        var locationCasa = new Location(19.617355752814138, -99.28939833885762);
        var mapSpan = MapSpan.FromCenterAndRadius(location, Distance.FromKilometers(5));

        RutaMap.MoveToRegion(mapSpan);

        // Puedes agregar pines así:
        var pin = new Pin {
            Label = "Punto de inicio",
            Location = location
        };
        RutaMap.Pins.Add(pin);

        var pin2 = new Pin {
            Label = "Punto final",
            Location = locationCasa
        };
        RutaMap.Pins.Add(pin2);
    }

    private async void Frame_Tapped(object sender, TappedEventArgs e) {
        Frame selectedFrame = (Frame)sender;
        selectedFrame.BackgroundColor = Color.FromArgb("#FFC8C8C8");
        await Task.Delay(100);
        selectedFrame.BackgroundColor = Color.FromArgb("#FFFFFFFF");
    }
}