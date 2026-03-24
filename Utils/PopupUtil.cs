using BatiaSuite.Models.OrdenesTrabajo;
using BatiaSuite.Popups;
using Mopups.Services;

namespace BatiaSuite.Utils;

public static class PopupUtil {

    public static async Task<MaterialRequest> GetMaterialesAsync() {

        MaterialRequest response = null;

        MatSumClientePopup popup = new MatSumClientePopup();

        await MopupService.Instance.PushAsync(popup);

        popup.viewModel.SendMaterial += (sender, e) => {
            response = e;
        };

        while(response is null) {
            await Task.Delay(100);
        }

        return response;
    }

    public static async Task<AlmacenModel> GetAlmacenAsync(AlmacenModel oldAlmacen) {
        AlmacenModel response = null;

        AlmacenPicker picker = new AlmacenPicker(oldAlmacen);

        await MopupService.Instance.PushAsync(picker);

        picker._viewModel.SelectedAlmacen += (sender, e) => {
            response = e;
        };

        while(response is null) {
            await Task.Delay(100);
        }

        return response;
    }

    public static async Task<MaterialResponse> GetMaterialAsync(int idAlmacen = 0, int idCliente = 0) {
        MaterialResponse response = null;

        MaterialPicker picker = new MaterialPicker(idAlmacen, idCliente);

        await MopupService.Instance.PushAsync(picker);

        picker._viewModel.SelectedMaterial += (sender, e) => {
            response = e;
        };

        while(response is null) {
            await Task.Delay(100);
        }

        return response;
    }

    public static async Task<PersonalOrdenTrabajoResponse> GetPersonalAsync() {
        PersonalOrdenTrabajoResponse response = null;

        PersonalPicker picker = new PersonalPicker();

        await MopupService.Instance.PushAsync(picker);

        picker._viewModel.SendPersonal += (sender, e) => {
            response = e;
        };

        while(response is null) {
            await Task.Delay(100);
        }

        return response;
    }

    public static async Task<object> GetObjectAsync(object oldValue, List<object> list, double divisor = 4, bool showSearching = false) {
        object response = null;

        ObjectPicker picker = new ObjectPicker(oldValue, list, divisor, showSearching);

        picker._viewModel.SendValue += (sender, e) => {
            response = e;
        };

        await MopupService.Instance.PushAsync(picker);

        while(response is null) {
            await Task.Delay(100);
        }

        return response;
    }

    public static async Task<bool> HasCameraPermissions() {
        try {
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.Camera>();

            if(status != PermissionStatus.Granted) {

                if(Permissions.ShouldShowRationale<Permissions.Camera>()) {
                    await App.Current.MainPage.DisplayAlert(Constants.GRANT_PERMISSIONS, Constants.PERMISSIONS_ERROR_CAM, Constants.ACEPTAR);
                }

                PermissionStatus permissionStatus = await Permissions.RequestAsync<Permissions.Camera>();

                if(permissionStatus != PermissionStatus.Granted) {
                    await App.Current.MainPage.DisplayAlert(Constants.GRANT_PERMISSIONS, Constants.PERMISSIONS_CAM_CONFIG, Constants.ACEPTAR);
                    return false;
                }
            }
        } catch(Exception) {
            return false;
        }
        return true;
    }
}