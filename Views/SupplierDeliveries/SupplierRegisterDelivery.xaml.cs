using BatiaSuite.ViewModel.SupplierDeliveries;

namespace BatiaSuite.Views.SupplierDeliveries;
public partial class SupplierRegisterDelivery : ContentPage 
{
    private string PathFirma;
    SupplierRegisterDeliveryViewModel supplierRegisterDeliveryViewModel;

    //public SupplierRegisterDelivery() {
    //    InitializeComponent();
    //    supplierRegisterDeliveryViewModel = new SupplierRegisterDeliveryViewModel(drawingView);
    //    BindingContext = supplierRegisterDeliveryViewModel;
    //}

    //// Método para asignar el MediaPicker después de crear la página
    //public void SetMediaPicker(IMediaPicker mediaPicker) {
    //    supplierRegisterDeliveryViewModel.mediaPicker = mediaPicker;
    //}

    public SupplierRegisterDelivery(IMediaPicker mediaPicker) {
        InitializeComponent();
        supplierRegisterDeliveryViewModel = new SupplierRegisterDeliveryViewModel(drawingView);
        supplierRegisterDeliveryViewModel.mediaPicker = mediaPicker;
        BindingContext = supplierRegisterDeliveryViewModel;
    }

    private async void btn_Firmar_Clicked(object sender, EventArgs e) {
    }

    private void Button_Clicked(object sender, EventArgs e) 
        {
        supplierRegisterDeliveryViewModel.IsSignature = true;
    }
}